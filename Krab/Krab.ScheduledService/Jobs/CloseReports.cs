using System;
using System.Linq;
using Krab.DataAccess.Dac;
using Krab.Logger;
using NCron;

namespace Krab.ScheduledService.Jobs
{
    public interface ICloseReports : ICronJob
    {

    }

    public class CloseReports : CronJob, ICloseReports
    {
        private readonly ILogger _logger;
        private readonly IKeywordResponseSetSubredditReportDac _dac;

        public CloseReports(ILogger logger, IKeywordResponseSetSubredditReportDac dac)
        {
            _logger = logger;
            _dac = dac;
        }

        public override void Execute()
        {
            _logger.LogInfo($"Executing {GetType()}.");

            try
            {
                foreach (var rpt in _dac.GetReadyToClose().ToList())
                {
                    try
                    {
                        _dac.Close(rpt.KeywordResponseSetId, rpt.SubredditId, rpt.ReportDateUtc);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to close report: {rpt.KeywordResponseSetId}:{rpt.SubredditId}:{rpt.ReportDateUtc}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType()} Failed.", ex);
            }

            _logger.LogInfo($"{GetType()} Complete.");
        }
    }
}
