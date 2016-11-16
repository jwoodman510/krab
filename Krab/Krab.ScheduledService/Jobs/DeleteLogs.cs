using System;
using System.IO;
using Krab.Global.Extensions;
using NCron;
using Krab.Logger;

namespace Krab.ScheduledService.Jobs
{
    public interface IDeleteLogs : ICronJob
    {
        
    }

    public class DeleteLogs : CronJob, IDeleteLogs
    {
        private const int DeleteDaysOlderThan = 7;

        private readonly ILogger _logger;
        private readonly string[] _directories =
        {
            @"c:\tmp\logs\ScheduledService",
            @"c:\tmp\logs\KeywordResponseSetProcessorService",
            @"c:\tmp\logs\KeywordResponseSetReportingService"
        };

        public DeleteLogs(ILogger logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _directories.ForEach(ProcessDirectory);
        }

        private void ProcessDirectory(string directory)
        {
            _logger.LogInfo($"Executing {GetType()}.");

            if (!Directory.Exists(directory))
            {
                _logger.LogInfo($"Directory not found: {directory}");
                return;
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                var fileInfo = new FileInfo(file);

                var cutoff = DateTime.UtcNow.AddDays(DeleteDaysOlderThan * -1);
                if (fileInfo.CreationTimeUtc >= cutoff)
                    continue;

                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to delete file: {file}.", ex);
                }
            }

            _logger.LogInfo($"{GetType()} Complete.");
        }
    }
}
