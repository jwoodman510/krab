using log4net;
using NCron;

namespace Krab.ScheduledService.Jobs
{
    public interface IProcessKeywordResponseSets : ICronJob
    {
        
    }

    public class ProcessKeywordResponseSets : CronJob, IProcessKeywordResponseSets
    {
        private readonly ILog _logger;

        public ProcessKeywordResponseSets(ILog logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.Info($"Executing {GetType()}.");

            //todo: process keyword response sets...

            _logger.Info($"{GetType()} Complete.");
        }
    }
}
