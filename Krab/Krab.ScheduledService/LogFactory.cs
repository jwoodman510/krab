using System;
using NCron;
using NCron.Logging;

namespace Krab.ScheduledService
{
    public class LogFactory : ILogFactory
    {
        public ILog GetLogForJob(ICronJob job)
        {
            return new CronLogger();
        }

        internal class CronLogger : ILog
        {
            public void Dispose()
            {

            }

            public void Debug(Func<string> msgCallback)
            {

            }

            public void Debug(Func<string> msgCallback, Func<Exception> exCallback)
            {

            }

            public void Info(Func<string> msgCallback)
            {

            }

            public void Info(Func<string> msgCallback, Func<Exception> exCallback)
            {

            }

            public void Warn(Func<string> msgCallback)
            {

            }
            
            public void Warn(Func<string> msgCallback, Func<Exception> exCallback)
            {

            }

            public void Error(Func<string> msgCallback)
            {

            }

            public void Error(Func<string> msgCallback, Func<Exception> exCallback)
            {

            }
        }
    }
}
