using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SE.PopCom.Host
{
    public class ConsumerService : IHostedService, IDisposable
    {
        private CancellationTokenSource cancelationTokerSource;
        private Task executingTask;
        private readonly SubscriberConfig subscriberConfig;
        private readonly AppConfig appConfig;
        private IConnection connection = null;
        private static AMQPSession amqpSession = null;
        private ConnectionFactory factory = new ConnectionFactory();
        private static ushort perfetchCount = 1;
        private static ConcurrentDictionary<string, IProcessMesssageHandler> handlerMap = new ConcurrentDictionary<string, IProcessMesssageHandler>();
        private static Dictionary<string, List<AMQPReceiverContext>> rcvDic = new Dictionary<string, List<AMQPReceiverContext>>();
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ConsumerService));

        public ConsumerService(IOptions<SubscriberConfig> subscriberConfig, IOptions<AppConfig> appConfig)
        {
            this.subscriberConfig = subscriberConfig.Value;
            this.appConfig = appConfig.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancelationTokerSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            executingTask = Task.Run(async () =>
            {
              
                try
                {
                    logger.Debug("test info");
                    amqpSession = new AMQPSession(appConfig, subscriberConfig);
                    amqpSession.RegisterReceiver();
                    if (amqpSession.IsConnected)
                    {
                        amqpSession.Start(false, amqpSession.RunningConsumerCount);
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal($"MessageConsumerService Start error: {ex.Message}");
                }
            });

            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
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

       
    }
}
