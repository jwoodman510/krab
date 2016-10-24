using log4net;
using System;
using System.IO;

namespace Krab.Logger
{
    public class KrabLogger : ILogger
    {
        private readonly ILog _logger;

        public KrabLogger()
        {
            var log4NetConfig = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));

            log4net.Config.XmlConfigurator.ConfigureAndWatch(log4NetConfig);

            _logger = LogManager.GetLogger("KrabLogger");
        }

        public void LogError(string error, Exception ex)
        {
            _logger.Error(error, ex);
        }
        
        public void LogInfo(string info)
        {
            _logger.Info(info);
        }

        public void LogWarning(string warning)
        {
            _logger.Warn(warning);
        }
    }
}
