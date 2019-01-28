using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.PopCom.Host
{
    public class TopicInfo
    {
        public byte QOS { get; set; }

        public string Topic { get; set; }

        public string RouteKey
        {
            get
            {
                return this.Topic.Replace("/", ".").Replace("+", "*");
            }
        }

        public string UniqueName
        {
            get
            {
                return GetSecondComponentOfTopic(this.Topic);
            }
        }

        /// <summary>
        /// Get the last component of topic
        /// CAUTION: If split topic with '/' fails, this method will return null.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static string GetLastComponentOfTopic(string topic)
        {
            var temp = GetTopicComponents(topic);
            var component = temp != null ? temp.LastOrDefault() : null;

            return component;
        }

        /// <summary>
        /// Get the second component of topic.
        /// CAUTION: If topic has no second part, this method will return null.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static string GetSecondComponentOfTopic(string topic)
        {
            var temp = TopicInfo.GetTopicComponents(topic);
            var component = temp != null && temp.Length >= 2 ? temp[1] : null;

            return component;
        }

        public static string[] GetTopicComponents(string topic)
        {
            var comps = topic.Contains("/") ? topic.Split('/') : null;

            return comps;
        }
    }
}
