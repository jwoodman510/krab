using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Krab.DataAccess.Dac;
using Krab.ScheduledService.Boostrap;
using Krab.ScheduledService.Jobs;
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
            TryGetInstances();

            if (_schedulingService == null)
            {
                _schedulingService = new SchedulingService
                {
                    LogFactory = new LogFactory()
                };
            }

            ScheduleJobs();

            _schedulingService.Start();

            _logger.Info("Service is started!");
        }

        private static void ScheduleJobs()
        {
            _logger.Info("Scheduling jobs...");

            var runKrJobEveryMin = Convert.ToInt32(ConfigurationManager.AppSettings["ProcessSetsEveryMinutes"]);

            if (runKrJobEveryMin == 1)
            {
                _schedulingService.At("* * * * *").Run(() => ServiceLocator.Current.GetInstance<IProcessKeywordResponseSets>());
                _logger.Info("Running IProcessKeywordResponseSets every minute.");
            }
            else if (runKrJobEveryMin > 1 && runKrJobEveryMin < 60)
            {
                _schedulingService.At($"*/{runKrJobEveryMin} * * * *").Run(() => ServiceLocator.Current.GetInstance<IProcessKeywordResponseSets>());
                _logger.Info($"Running IProcessKeywordResponseSets every {runKrJobEveryMin} minutes.");
            }
            else
            {
                _logger.Warn($"Invalid AppSetting: key=ProcessSetsEveryMinutes value={runKrJobEveryMin}");
            }

            _schedulingService.Daily().Run(() => ServiceLocator.Current.GetInstance<IDeleteLogs>());
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

        private static void TryGetInstances()
        {
            var locator = ServiceLocator.Current;

            if (locator == null)
            {
                _logger.Error("Unable to find Service Locator!");
                return;
            }

            var types = new List<Type>
            {
                typeof(IDeleteLogs),
                typeof(IProcessKeywordResponseSets)
            };

            foreach (var type in types)
            {
                try
                {
                    locator.GetInstance(type);
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Unable to find instane of {type.Name}!");
                    _logger.Error($"Unable to find instane of {type.Name}!", ex);

                    #if DEBUG
                        throw;
                    #endif
                }
            }
        }
    }
}
