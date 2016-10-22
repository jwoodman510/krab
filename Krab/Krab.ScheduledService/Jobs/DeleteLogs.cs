using System;
using System.IO;
using log4net;
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
        private const string Directory = @"c:\tmp\logs\ScheduledService";

        private readonly ILogger _logger;

        public DeleteLogs(ILogger logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.LogInfo($"Executing {GetType()}.");

            if (!System.IO.Directory.Exists(Directory))
            {
                _logger.LogInfo($"Directory not found: {Directory}");
                return;
            }

            foreach (var file in System.IO.Directory.GetFiles(Directory))
            {
                var fileInfo = new FileInfo(file);

                if (DateTime.UtcNow.AddDays(DeleteDaysOlderThan) <= fileInfo.CreationTimeUtc)
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
