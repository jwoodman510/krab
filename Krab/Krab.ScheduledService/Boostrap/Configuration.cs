using Krab.ScheduledService.Jobs;
using Microsoft.Practices.Unity;

namespace Krab.ScheduledService.Boostrap
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            RegisterJobs(container);
        }

        private static void RegisterJobs(IUnityContainer container)
        {
            container.RegisterType<IProcessKeywordResponseSets, ProcessKeywordResponseSets>();
            container.RegisterType<IDeleteLogs, DeleteLogs>();
            container.RegisterType<ICloseReports, CloseReports>();
        }
    }
}
