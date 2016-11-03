using System;
using System.Configuration;
using Krab.ScheduledService.Boostrap;
using Krab.ScheduledService.Jobs;
using Microsoft.Practices.ServiceLocation;
using NCron.Fluent.Crontab;
using NCron.Service;
using Topshelf;
using Krab.Logger;

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
            private static ILogger _logger;
            private static SchedulingService _schedulingService;

            public void Start()
            {
                Bootstrapper.Configure();

                _logger = ServiceLocator.Current.GetInstance<ILogger>();

                _logger.LogInfo("Starting service...");

                if (_schedulingService == null)
                {
                    _schedulingService = new SchedulingService
                    {
                        LogFactory = new LogFactory()
                    };
                }

                ScheduleJobs();

                _schedulingService.Start();

                _logger.LogInfo("Service is started!");
            }

            public void ScheduleJobs()
            {
                _logger.LogInfo("Scheduling jobs...");

                var runKrJobEveryMin = Convert.ToInt32(ConfigurationManager.AppSettings["ProcessSetsEveryMinutes"]);

                if (runKrJobEveryMin == 1)
                {
                    _schedulingService.At("* * * * *").Run(() => ServiceLocator.Current.GetInstance<IProcessKeywordResponseSets>());
                    _logger.LogInfo("Running IProcessKeywordResponseSets every minute.");
                }
                else if (runKrJobEveryMin > 1 && runKrJobEveryMin < 60)
                {
                    _schedulingService.At($"*/{runKrJobEveryMin} * * * *").Run(() => ServiceLocator.Current.GetInstance<IProcessKeywordResponseSets>());
                    _logger.LogInfo($"Running IProcessKeywordResponseSets every {runKrJobEveryMin} minutes.");
                }
                else
                {
                    _logger.LogWarning($"Invalid AppSetting: key=ProcessSetsEveryMinutes value={runKrJobEveryMin}");
                }

                _schedulingService.Daily().Run(() => ServiceLocator.Current.GetInstance<IDeleteLogs>());
            }

            public void Stop()
            {
                _logger.LogInfo("Stopping service...");
                _schedulingService?.Stop();
                _schedulingService?.Dispose();
                _logger.LogInfo("Service Stopped.");
            }
        }
    }
}
