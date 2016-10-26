using System;

namespace Krab.Logger
{
    public class EmptyLogger : ILogger
    {
        public void LogInfo(string info)
        {

        }

        public void LogWarning(string warning)
        {

        }

        public void LogError(string error, Exception ex)
        {

        }
    }
}
