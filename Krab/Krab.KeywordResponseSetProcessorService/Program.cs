using log4net;
using System;
using System.IO;
using Topshelf;

namespace Krab.KeywordResponseSetProcessorService
{
    public class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.New(x =>
            {
                x.Service<Service>(svc =>
                {
                    svc.ConstructUsing(s => new Service());
                    svc.WhenStarted(s => s.Start());
                    svc.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("This service receives incoming messages regarding keyword-response sets.");
                x.SetDisplayName("KRAB Keyword-Response Set Processor Service");
                x.SetServiceName("Krab.KeywordResponseSetProcessorService");
                x.StartAutomatically();

            }).Run();
        }

        public class Service
        {
            private static ILog _logger;

            public void Start()
            {
                SetupLogger();

                _logger.Info("Starting service...");

                Bootstrap.Bootstrapper.Configure();

                _logger.Info("Service is started!");
            }

            private static void SetupLogger()
            {
                var log4NetConfig = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));

                log4net.Config.XmlConfigurator.ConfigureAndWatch(log4NetConfig);

                _logger = LogManager.GetLogger("ServiceLogger");
            }

            public void Stop()
            {
                _logger.Info("Stopping service...");

                Bootstrap.Bootstrapper.StopReceiveBus();

                _logger.Info("Service Stopped.");
            }
        }
    }
}
