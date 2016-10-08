using System;
using System.ServiceProcess;
using Krab.ScheduledService.Boostrap;
using Microsoft.Practices.ServiceLocation;
using NCron.Fluent.Crontab;
using NCron.Logging;
using NCron.Service;

namespace Krab.ScheduledService
{
    public class Program
    {
        public const string ServiceName = "Krab.ScheduledService";
        private static SchedulingService _schedulingService;

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
        }

        private static void Stop()
        {
            _schedulingService?.Stop();
        }
    }
}
