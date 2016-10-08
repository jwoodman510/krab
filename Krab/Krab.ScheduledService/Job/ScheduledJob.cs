using log4net;
using NCron;

namespace Krab.ScheduledService.Job
{
    public interface IScheduledJob : ICronJob
    {
        
    }

    public class ScheduledJob : CronJob, IScheduledJob
    {
        private readonly ILog _logger;

        public ScheduledJob(ILog logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.Info($"Executing {GetType()}.");
        }
    }
}
