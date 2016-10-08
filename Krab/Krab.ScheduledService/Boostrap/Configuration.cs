using Krab.ScheduledService.Job;
using Microsoft.Practices.Unity;

namespace Krab.ScheduledService.Boostrap
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IScheduledJob, ScheduledJob>();
        }
    }
}
