using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SE.PopCom.Host
{
    public class SubscriberConfig
    {
        public Subscriber[] Subscribers { get; set; }
    }
    public class Subscriber
    {
        /// <summary>
        /// Subscriber Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Subscriber Type
        /// </summary>
        public string Type { get; set; }
    }
  
}
