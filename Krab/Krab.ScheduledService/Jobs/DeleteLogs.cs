using System;
using System.IO;
using log4net;
using NCron;
namespace Krab.ScheduledService.Jobs
{
    public interface IDeleteLogs : ICronJob
    {
        
    }

    public class DeleteLogs : CronJob, IDeleteLogs
    {
        private const int DeleteDaysOlderThan = 7;
        private const string Directory = @"c:\tmp\logs\ScheduledService";

        private readonly ILog _logger;

        public DeleteLogs(ILog logger)
        {
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.Info($"Executing {GetType()}.");

            if (!System.IO.Directory.Exists(Directory))
            {
                _logger.Info($"Directory not found: {Directory}");
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
                    _logger.Error($"Unable to delete file: {file}.", ex);
                }
            }

            _logger.Info($"{GetType()} Complete.");
        }
    }
}
