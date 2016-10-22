using System;

namespace Krab.Logger
{
    public interface ILogger
    {
        void LogInfo(string info);

        void LogWarning(string warning);

        void LogError(string error, Exception ex);
    }
}
