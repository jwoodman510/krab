using Microsoft.Practices.Unity;

namespace Krab.ScheduledService
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IScheduledJob, ScheduledJob>();
        }
    }
}
