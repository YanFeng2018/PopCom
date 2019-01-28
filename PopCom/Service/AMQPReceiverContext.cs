using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SE.PopCom.Host.Business;

namespace SE.PopCom.Host
{
    public class AMQPReceiverContext : IDisposable
    {
        /// <summary>
        /// 允许同时开启Consume的最大个数
        /// </summary>
        private const int MaxConsumeNumber = 1000;
        private bool autoAck = true;
        private IConnection connection = null;
        private Dictionary<string, IBasicConsumer> consumeDic = new Dictionary<string, IBasicConsumer>();
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AMQPReceiverContext));

        /// <summary>
        /// 读取数据队列名称
        /// </summary>
        public string QueueName
        {
            get;
            private set;
        }

        public event EMOPMessageAction<IncomingMsgEventArgs> OnReceived;

        public string ReceiverName
        {
            get
            {
                return string.Format("{0}-{1}", this.QueueName, this.connection != null ? this.connection.ClientProvidedName : string.Empty);
            }
        }

        public AMQPReceiverContext(IConnection connection, string ququeName)
        {
            this.connection = connection;
            this.QueueName = ququeName;
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="autoAck"></param>
        public void Start(bool autoAck, ushort prefetchCount = 10, int minConsumeCount = 1)
        {
            this.autoAck = autoAck;
            if (minConsumeCount <= 0)
            {
                minConsumeCount = 1;
                logger.Warn("receiving message min consume number is 1");
            }
            else if (minConsumeCount > MaxConsumeNumber)
            {
                minConsumeCount = MaxConsumeNumber;
                logger.Warn($"for one model, the receiving message max consume number is {MaxConsumeNumber}");
            }

            this.consumeDic.Clear();
            for (int i = 1; i <= minConsumeCount; i++)
            {
                var model = this.connection.CreateModel();
                model.ModelShutdown += this._model_ModelShutdown;
                model.CallbackException += this._model_CallbackException;
                model.BasicQos(0, prefetchCount, false);
                logger.Info($"register an new model[{model.ChannelNumber}] at connection[{this.connection.ClientProvidedName}]");
                var consumer = new EventingBasicConsumer(model);
                consumer.Registered += this._consumer_Registered;
                consumer.ConsumerCancelled += this._consumer_ConsumerCancelled;
                consumer.Received += this._consumer_Received;
                string consumerTag = string.Empty;
                try
                {
                    uint ccount = model.ConsumerCount(this.QueueName);
                    ccount++;
                    consumerTag = string.Format("ctag_{0}_{1}", this.QueueName, ccount);
                    var ctag = model.BasicConsume(this.QueueName, autoAck, consumerTag, consumer);
                    this.consumeDic.Add(consumerTag, consumer);
                    logger.Info($"start an new consumer[{ctag}] at model[{model.ChannelNumber}] for receiving message");
                }
                catch (Exception ex)
                {
                    logger.Error($"start new consumer[{consumerTag}] with error");
                    logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// 停止接收消息
        /// </summary>
        public void Stop()
        {
            foreach (var consume in this.consumeDic)
            {
                var channelNumber = consume.Value.Model.ChannelNumber;
                consume.Value.Model.Abort();
                consume.Value.Model.Dispose();
                logger.Info($"receive model[{channelNumber}] consume[{consume.Key}] Abort and Dispose");
            }
        }

        public void SetConnection(IConnection conn)
        {
            this.connection = conn;
        }

        public void Dispose()
        {
            foreach (var consume in this.consumeDic)
            {
                var channelNumber = consume.Value.Model.ChannelNumber;
                if (!consume.Value.Model.IsClosed)
                {
                    consume.Value.Model.Close();
                }

                consume.Value.Model.Dispose();
                logger.Info($"receive model[{channelNumber}] consume[{consume.Key}] Abort and Dispose");
            }
        }

        private void _model_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            int channelNumer = ((IModel)sender).ChannelNumber;
            logger.Error($"receiver model[{channelNumer}] CallbackException");
            logger.Error(e.Exception);
        }

        private void _model_ModelShutdown(object sender, ShutdownEventArgs e)
        {
            int channelNumer = ((IModel)sender).ChannelNumber;
            logger.Info($"receiver model[{channelNumer}] was Shutdown {e.ReplyCode} {e.ReplyText}");
        }

        private void _consumer_ConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            var channelNumber = ((IBasicConsumer)sender).Model.ChannelNumber;
            logger.Info($"receive model[{channelNumber}] consumer[{e.ConsumerTag}] was cancelled");
        }

        private void _consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var args = new IncomingMsgEventArgs(e.RoutingKey, e.Body, e.Redelivered);
            logger.Debug($"RCV {e.ConsumerTag} {e.RoutingKey} {e.DeliveryTag} @{args.Body}");

            bool? rst = default(bool?);
            args.ConsumerTag = e.ConsumerTag;
            args.DeliveryTag = e.DeliveryTag;
            rst = this.OnReceived(args);

            if (!this.autoAck)
            {
                var model = ((IBasicConsumer)sender).Model;
                try
                {
                    if (!model.IsClosed && model.IsOpen)
                    {
                        if (rst.HasValue && rst.Value)
                        {
                            model.BasicReject(e.DeliveryTag, false);        ////当确定调用业务方法处理消息失败时，不会重新加入队列，为死信做准备
                            logger.Debug($"{ e.RoutingKey} {e.DeliveryTag} reject to quque");
                        }
                        else
                        {
                            model.BasicAck(e.DeliveryTag, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("model[{model.ChannelNumber}] ack {e.RoutingKey} message[{e.DeliveryTag}] exception");
                    logger.Error(ex);
                }
            }
        }

        private void _consumer_Registered(object sender, ConsumerEventArgs e)
        {
            logger.Info($"receive consumer[{e.ConsumerTag}] is registered ");
        }
    }
}
