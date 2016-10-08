using System;
using System.IO;
using System.ServiceProcess;
using Krab.ScheduledService.Boostrap;
using Krab.ScheduledService.Job;
using log4net;
using Microsoft.Practices.ServiceLocation;
using NCron.Fluent.Crontab;
using NCron.Service;

namespace Krab.ScheduledService
{
    public class Program
    {
        public const string ServiceName = "Krab.ScheduledService";

        private static SchedulingService _schedulingService;

        private static ILog _logger;

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }

        private static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                using (var service = new Service())
                {
                    ServiceBase.Run(service);
                }
            }
            else
            {
                Start(args);

                Console.WriteLine("Press escape to stop the service.");

                var key = Console.ReadKey(true);
                while (key.Key != ConsoleKey.Escape)
                {
                    key = Console.ReadKey(true);
                }

                Stop();
            }
        }

        private static void Start(string[] args)
        {
            SetupLogger();

            _logger.Info("Starting service...");

            Bootstrapper.Configure();

            if (_schedulingService == null)
            {
                _schedulingService = new SchedulingService
                {
                    LogFactory = new LogFactory()
                };
            }

            _schedulingService.At("* * * * *").Run(() => ServiceLocator.Current.GetInstance<IScheduledJob>());

            _schedulingService.Start();


            _logger.Info("Service is started!");
        }

        private static void Stop()
        {
            _logger.Info("Stopping service...");
            _schedulingService?.Stop();
            _logger.Info("Service Stopped.");
        }

        private static void SetupLogger()
        {
            var log4NetConfig = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));

            if (!log4NetConfig.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to find log4net.config!");
                Console.ResetColor();
            }

            log4net.Config.XmlConfigurator.ConfigureAndWatch(log4NetConfig);

            _logger = LogManager.GetLogger("ServiceLogger");
        }
    }
}
