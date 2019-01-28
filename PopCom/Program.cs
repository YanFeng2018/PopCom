using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SE.PopCom.DataAccess;

namespace SE.PopCom.Host
{
    class Program
    {
        public static async Task Main(string[] args)
        {
           
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("Config/AppSettings.json", optional: true);
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddOptions();
                    services.Configure<SubscriberConfig>(hostContext.Configuration.GetSection("SubscriberConfig"));
                    services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                    services.AddHostedService<ConsumerService>();
                    services.AddSingleton<DataAccessBase>();

                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    var log4netConfigFilePath = hostContext.Configuration.GetSection("log4net");
                    configLogging.UseLogHelper("Config/log4net.config");


                })
                .UseConsoleLifetime()
                .Build();
            var settingCfg = host.Services.GetService<IOptions<AppConfig>>().Value;
            host.UseInitDataAccessBase(settingCfg.DBConnectionString);
            await host.RunAsync();
        }
    }
}
