using Krab.Logger;
using Microsoft.Practices.ServiceLocation;
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
            private static ILogger _logger;

            public void Start()
            {
                Bootstrap.Bootstrapper.Configure();

                _logger = ServiceLocator.Current.GetInstance<ILogger>();

                _logger.LogInfo("Service is started!");
            }

            public void Stop()
            {
                _logger.LogInfo("Stopping service...");

                Bootstrap.Bootstrapper.StopReceiveBus();

                _logger.LogInfo("Service Stopped.");
            }
        }
    }
}
