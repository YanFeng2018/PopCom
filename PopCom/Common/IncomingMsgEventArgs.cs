using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host
{
    public class IncomingMsgEventArgs
    {
        public string TopicName { get; private set; }

        public string Version { get; private set; }

        public string BoxId { get; private set; }

        /// <summary>
        /// 当实时数据或状态数据时，才会有DataTopic的值
        /// </summary>
        public string DataTopic { get; private set; }

        public string Body { get; private set; }

        public string TopicFullOrRoutingKey { get; private set; }

        public byte Qos { get; set; }

        public string ConsumerTag { get; set; }

        public ulong DeliveryTag { get; set; }

        /// <summary>
        /// 是否重复消息
        /// </summary>
        public bool IsDuplicate { get; private set; }

        public IncomingMsgEventArgs(string realTopicOrRoutingKey, byte[] message, bool isDuplicate)
        {
            this.TopicFullOrRoutingKey = realTopicOrRoutingKey;
            this.IsDuplicate = isDuplicate;
            this.Body = DecodeMessage(message);

            var comps = realTopicOrRoutingKey.Replace("/", ".").Replace("+", "*").Split('.');
            this.Version = comps.Length >= 2 ? comps[0] + "." + comps[1] : string.Empty;
            this.TopicName = comps.Length >= 3 ? comps[2] : string.Empty;
            this.BoxId = comps.Length >= 4 ? comps[3] : string.Empty;
            this.DataTopic = comps.Length >= 5 ? comps[4] : string.Empty;
        }

        private static string DecodeMessage(byte[] message)
        {
            if (message != null && message.Length > 0)
            {
                var body = new byte[message.Length];
                Array.Copy(message, 0, body, 0, body.Length);
                var result = Encoding.UTF8.GetString(body);
                return result;
            }

            return string.Empty;
        }
    }
}
