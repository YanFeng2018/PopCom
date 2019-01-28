using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host
{
    public class AppConfig
    {
        /// <summary>
        /// SQL Server Connection String
        /// </summary>
        public string DBConnectionString { get; set; }
        
        /// <summary>
        /// MQ Server URI 
        /// </summary>
        public string BrokerAMQPURI { get; set; }

        /// <summary>
        /// MQ Server URI 
        /// </summary>
        public string MQClientId { get; set; }

        /// <summary>
        /// minum consumer count
        /// </summary>
        public int AMQPMinConsumerCount { get; set; }

        /// <summary>
        /// queue list, splited with '|'
        /// </summary>
        public string AMQPQueueList { get; set; }
    }
}
