using System;

namespace SE.PopCom.Host
{
    public abstract class MessageHandlerBase : IProcessMesssageHandler
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MessageHandlerBase));

        public abstract TopicInfo SubscribingTopic
        {
            get;
        }
        public string Protocol { get { return "AMQP"; } }

        public virtual void Registered()
        {
        }

        /// <summary>
        /// 处理消息句柄接口
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// 返回值为true，则指示消息需要加入队列被重新消费处理
        /// </returns>
        public bool? ReceiveHandler(IncomingMsgEventArgs args)
        {
            if (args.IsDuplicate)
            {
                logger.Debug($"RCV {args.ConsumerTag} {args.TopicFullOrRoutingKey} {args.DeliveryTag} IsDuplicate, no process @{args.Body}");
                return default(bool?);
            }

                return this.ProcessWrapperReturn(args);
           
        }

        /// <summary>
        /// 处理消息句柄接口
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// 当Protocol是AMQP时，返回值为true，则指示消息需要加入队列被重新消费处理
        /// 默认请返回 false 表示消息处理完成，不需要重复处理
        /// </returns>
        protected abstract bool? Process(IncomingMsgEventArgs args);

        private bool? ProcessWrapperReturn(object args)
        {
            bool? rst = default(bool?);
            try
            {
                var msgArgs = (IncomingMsgEventArgs)args;

                DateTime dtbegin = DateTime.Now;

                rst = this.Process(msgArgs);

                TimeSpan timespan = DateTime.Now - dtbegin;

                logger.Debug(string.Format($"PROC {msgArgs.TopicFullOrRoutingKey} {msgArgs.DeliveryTag} result: ok {timespan.TotalMilliseconds}"));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return rst;
        }
    }
}
