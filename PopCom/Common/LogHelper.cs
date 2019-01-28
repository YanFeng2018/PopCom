using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SE.PopCom.Host
{
    public static class LogHelper
    {

        public static ILoggingBuilder UseLogHelper(this ILoggingBuilder loggingBuilder, string configFilePath)
        {
            // Configure Log
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(configFilePath));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
            return loggingBuilder;
        }
    }
}
