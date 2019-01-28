using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host
{
    public delegate bool? EMOPMessageAction<in T>(T obj);

    public interface IProcessMesssageHandler
    {
        TopicInfo SubscribingTopic { get; }

        String Protocol { get; }

        void Registered();

        /// <summary>
        /// 处理消息句柄接口
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// 当Protocol是AMQP时，返回值为true，则指示消息需要加入死信队列
        /// </returns>
        bool? ReceiveHandler(IncomingMsgEventArgs args);
    }
}
