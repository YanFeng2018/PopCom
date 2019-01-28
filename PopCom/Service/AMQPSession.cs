using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.PopCom.Host
{
    public class AMQPSession : IDisposable
    {
        private static ushort heartbeatTimeout = 45;
        private static ushort networkRecoveryInterval = 57;
        private static ushort networkRecoveryIntervalMax = 300;
        private static ushort perfetchCount = 1;
        private AppConfig appConfig;
        private SubscriberConfig subscriberConfig;
        private static Dictionary<string, List<AMQPReceiverContext>> rcvDic = new Dictionary<string, List<AMQPReceiverContext>>();
        private static ConcurrentDictionary<string, IProcessMesssageHandler> handlerMap = new ConcurrentDictionary<string, IProcessMesssageHandler>();
        private ConnectionFactory factory = new ConnectionFactory();
        private IConnection connection = null;
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AMQPSession));

        /// <summary>
        /// 定义一个用于断线重连的轻量级定时器, 默认60秒重连一次
        /// </summary>
        private System.Timers.Timer timerRetry = new System.Timers.Timer(networkRecoveryInterval * 1000);

        /// <summary>
        /// 连接次数
        /// </summary>
        private int connCount = 0;

        /// <summary>
        /// 用于发布消息的Model信道
        /// </summary>
        private IModel publishModel = null;

        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnName
        {
            get;
            private set;
        }

        /// <summary>
        /// 指示当前是否连接着
        /// </summary>
        public bool IsConnected
        {
            get;
            private set;
        }

        public int RunningConsumerCount
        {
            get;
            private set;
        }

        public static AMQPSession Instance(AppConfig appConfig, SubscriberConfig subscriberConfig)
        {
            return new AMQPSession(appConfig, subscriberConfig);
        }

        public AMQPSession(AppConfig appConfig, SubscriberConfig subscriberConfig, bool autoRecovery = true)
        {
            this.appConfig = appConfig;
            this.subscriberConfig = subscriberConfig;
            InitSession();
        }

        private void InitSession()
        {
            this.RunningConsumerCount = appConfig.AMQPMinConsumerCount;
            this.timerRetry.AutoReset = true;
            this.timerRetry.Elapsed += this.TimerRetry_Elapsed;
            this.timerRetry.Enabled = false;
            this.InitConnection(appConfig.BrokerAMQPURI, appConfig.MQClientId);
            this.InitProcessHandler(subscriberConfig);
        }
        /// <summary>
        /// 返回处理Handler类型的实例
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public IProcessMesssageHandler GetProcessHandler(Type handlerType)
        {
            IProcessMesssageHandler handler = null;
            if (handlerMap != null)
            {
                handler = handlerMap.Values.Where(x => handlerType.IsInstanceOfType(x)).FirstOrDefault();
            }

            return handler;
        }

        /// <summary>
        /// 注册队列
        /// </summary>
        public void RegisterReceiver()
        {
            var queueList = appConfig.AMQPQueueList;
            if (!string.IsNullOrEmpty(queueList))
            {
                foreach (var qname in queueList.Split('|'))
                {
                    this.RegisterReceiver(qname);
                }
            }
            else
            {
                logger.Error("将要消费的队列AMQPQueueList尚未配置");
            }
        }

        public AMQPReceiverContext RegisterReceiver(string queueName)
        {
            if (string.IsNullOrEmpty(queueName) || queueName.Length <= 0)
            {
                logger.Error("AMQPSession.RegisterReceiver QueueName is not allow null");
                throw new ArgumentNullException(queueName);
            }

            AMQPReceiverContext context = new AMQPReceiverContext(this.connection, queueName);
            context.OnReceived += this.Context_OnReceived;
            if (rcvDic.ContainsKey(queueName))
            {
                List<AMQPReceiverContext> rcvList = rcvDic[queueName];
                if (rcvList == null)
                {
                    rcvList = new List<AMQPReceiverContext>();
                }
                else
                {
                    foreach (var rcv in rcvList)
                    {
                        rcv.SetConnection(this.connection);
                    }
                }

                if (!rcvList.Exists(x => x.ReceiverName == context.ReceiverName))
                {
                    rcvList.Add(context);
                }
            }
            else
            {
                List<AMQPReceiverContext> rcvList = new List<AMQPReceiverContext>();
                rcvList.Add(context);
                rcvDic.Add(queueName, rcvList);
            }

            return context;
        }

        public void Start(bool autoAck, int minConsumeNumber = 5, string queueName = "")
        {
            if (string.IsNullOrWhiteSpace(queueName) || queueName.Length <= 0)
            {
                foreach (var rcvgroup in rcvDic)
                {
                    if (rcvgroup.Value != null && rcvgroup.Value.Count >= 1)
                    {
                        foreach (var rcv in rcvgroup.Value)
                        {
                            rcv.Start(autoAck, perfetchCount, minConsumeNumber);
                        }
                    }
                }
            }
            else if (rcvDic.ContainsKey(queueName))
            {
                if (rcvDic[queueName] != null && rcvDic[queueName].Count >= 1)
                {
                    foreach (var rcv in rcvDic[queueName])
                    {
                        rcv.Start(autoAck, perfetchCount, minConsumeNumber);
                    }
                }
            }
        }

        public void Stop(bool isCloseConn = true)
        {
            foreach (var rcvgroup in rcvDic)
            {
                if (rcvgroup.Value != null && rcvgroup.Value.Count >= 1)
                {
                    foreach (var rcv in rcvgroup.Value)
                    {
                        rcv.Stop();
                    }
                }
            }

            if (this.publishModel != null)
            {
                var channelNumber = this.publishModel.ChannelNumber;
                this.publishModel.Abort();
                this.publishModel.Dispose();
                logger.Info($"connection[{ this.ConnName}] publishModel[channelNumber] was Abort and dispose");
            }

            if (isCloseConn)
            {
                this.connection.Close();
                this.connection.Abort();
                this.connection.Dispose();

                logger.Info($"connection[{this.ConnName}] was Abort and dispose");

                if (this.timerRetry != null)
                {
                    this.timerRetry.Enabled = false;
                    this.timerRetry.Stop();
                    this.timerRetry.Close();
                    this.timerRetry.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                if (this.connection.IsOpen)
                {
                    this.connection.Close();
                }

                this.connection.Dispose();
            }
        }

        private void InitConnection(string brokerAmqpUri, string clientId, bool autoRecovery = true)
        {
            this.ConnName = this.GeneralConnName(clientId);

            if (this.factory == null)
            {
                this.factory = new ConnectionFactory();
            }
            this.factory.Uri = new Uri(brokerAmqpUri);
            this.factory.RequestedHeartbeat = heartbeatTimeout;
            this.factory.RequestedConnectionTimeout = 15000;
            this.factory.AutomaticRecoveryEnabled = autoRecovery;
            this.factory.NetworkRecoveryInterval = new TimeSpan(0, 0, 0, networkRecoveryInterval, 0);

            logger.Debug($"RabbitMQ host[{this.factory.HostName}:{this.factory.Port}] username:{this.factory.UserName} password:{this.factory.Password} virtualhost:{this.factory.VirtualHost}");

            this.ConnBroker();
            if (!this.IsConnected)
            {
                this.timerRetry.Enabled = true;
                this.timerRetry.Start();
                logger.Info($"connection[{this.ConnName}] to AMQP rabbitMQ failed and start timerRetry");
            }
            else
            {
                ////注册断线重连出现错误时的事件
                this.connection.ConnectionRecoveryError += this.Connection_ConnectionRecoveryError;
                ////注册连接关闭时的事件
                this.connection.ConnectionShutdown += this.Connection_ConnectionShutdown;
                ////注册重连成功时的事件
                this.connection.RecoverySucceeded += this.Connection_RecoverySucceeded;
                ////注册连接被阻止时的事件
                this.connection.ConnectionBlocked += this.Connection_ConnectionBlocked;
                ////注册回调异常时的事件
                this.connection.CallbackException += this.Connection_CallbackException;
            }
        }

        private bool ConnBroker(bool autoStart = false)
        {
            this.connCount++;
            try
            {
                this.connection = this.factory.CreateConnection(this.ConnName);
                this.IsConnected = true;
            }
            catch (Exception ex)
            {
                logger.Error($"connection[{this.ConnName}] exception at AMQP rabbitMQ retry[{this.connCount}]");
                logger.Error(ex);
                this.IsConnected = false;
            }

            if (this.IsConnected)
            {
                this.timerRetry.Enabled = false;
                this.timerRetry.Stop();
                logger.Info($"connection[{this.ConnName}] to AMQP rabbitMQ success[{this.connCount}] and stop timerRetry");

                if (autoStart)
                {
                    this.RegisterReceiver();

                    this.Start(false, this.RunningConsumerCount);
                }
            }
            else if (this.connCount >= 10)
            {
                ////如果重连的次数超过10次，则改为3分钟重试一次
                this.timerRetry.Interval = networkRecoveryIntervalMax * 1000;
                this.timerRetry.Enabled = true;
            }

            return this.IsConnected;
        }

        private void TimerRetry_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.connection == null || !this.connection.IsOpen)
            {
                this.ConnBroker(true);
            }
            else
            {
                this.timerRetry.Enabled = false;
                this.timerRetry.Stop();
            }
        }

        private void InitProcessHandler(SubscriberConfig subscriberConfig)
        {
            handlerMap.Clear();
            var processMapping = subscriberConfig.Subscribers.ToDictionary(k => k.Name, v => v.Type);

            foreach (var subscriber in processMapping)
            {
                var handler = (IProcessMesssageHandler)Activator.CreateInstance(Type.GetType(subscriber.Value));
                var uname = handler.SubscribingTopic.UniqueName;
                handler.Registered();
                handlerMap.TryAdd(uname, handler);
            }
        }
        
        private bool? Context_OnReceived(IncomingMsgEventArgs e)
        {
            bool? rst = false;
            //if (!string.IsNullOrEmpty(e.TopicName) && handlerMap.ContainsKey(e.TopicName))
            if(true)
            {
                // rst = handlerMap[e.TopicName].ReceiveHandler(e);
                rst = handlerMap["DiagnosticResult"].ReceiveHandler(e);
            }
            else
            {
                logger.Error($"RCV routingkey[{e.TopicFullOrRoutingKey}] and topicname[{e.TopicName}] is not config the process handler");
                rst = false;     ////无法识别的消息直接忽略, 目前尚未考虑死信队列
            }

            return rst;
        }

        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            logger.Error($"connection[{this.ConnName}] CallbackException");
            logger.Error(e.Exception);
        }

        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            logger.Error($"connection[{this.ConnName}] Blocked Reason[{e.Reason}]");
        }

        /// <summary>
        /// 断线重连成功后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this.timerRetry.Enabled = false;
            this.timerRetry.Stop();
            logger.Info($"connection[{this.ConnName}] RecoverySucceeded retry[{this.connCount}] and stop timerRetry");
        }

        /// <summary>
        /// 连接被关闭时的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            this.IsConnected = false;
            logger.Info($"connection[{this.ConnName}] Shutdown {e.ReplyCode} {e.ReplyText}");
            if (e.ReplyCode != 200)
            {
                this.timerRetry.Enabled = true;
                this.timerRetry.Start();
                logger.Info($"connection[{this.ConnName}] to AMQP rabbitMQ was Shutdown exception and start timerRetry");
            }
            else
            {
                this.timerRetry.Enabled = false;
                this.timerRetry.Stop();
                this.timerRetry.Close();
                logger.Info($"connection[{this.ConnName}] to AMQP rabbitMQ was Shutdown normally, stop and close timerRetry");
            }
        }

        /// <summary>
        /// 断线重连出现错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            logger.Error(e.Exception);
            this.timerRetry.Enabled = true;
            this.timerRetry.Start();
            logger.Info($"connection[{this.ConnName}] to AMQP rabbitMQ RecoveryError retry[{this.connCount}] and start timerRetry");
        }

        /// <summary>
        /// 生成自定义的连接名称
        /// </summary>
        /// <returns></returns>
        private string GeneralConnName(string clientId)
        {
            this.ConnName = string.Format("PopCom_Conn_{0}", clientId);
            return this.ConnName;
        }
    }
}
