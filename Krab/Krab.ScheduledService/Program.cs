using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Krab.ScheduledService.Boostrap;
using Krab.ScheduledService.Jobs;
using log4net;
using Microsoft.Practices.ServiceLocation;
using NCron.Fluent.Crontab;
using NCron.Service;
using Topshelf;

namespace Krab.ScheduledService
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
                x.SetDescription("This service runs scheduled jobs.");
                x.SetDisplayName("KRAB Scheduled Service");
                x.SetServiceName("Krab.ScheduledService");
                x.StartAutomatically();

            }).Run();
        }

        public class Service
        {
            private static ILog _logger;
            private static SchedulingService _schedulingService;

            public void Start()
            {
                SetupLogger();

                Bootstrapper.Configure();

                _logger.Info("Starting service...");

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

            private static void SetupLogger()
            {
                var log4NetConfig = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));

                log4net.Config.XmlConfigurator.ConfigureAndWatch(log4NetConfig);

                _logger = LogManager.GetLogger("ServiceLogger");
            }

            public void ScheduleJobs()
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

            public void Stop()
            {
                _logger.Info("Stopping service...");
                _schedulingService?.Stop();
                _schedulingService?.Dispose();
                _logger.Info("Service Stopped.");
            }
        }
    }
}
