using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Krab.ScheduledService.Boostrap
{
    public static class Bootstrapper
    {
        public static void Configure()
        {
            var container = new UnityContainer();

            RegisterInstances(container);

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }

        private static void RegisterInstances(IUnityContainer container)
        {
            Configuration.Register(container);
            DataAccess.Configuration.Register(container);
            Api.Configuration.Register(container);
        }
    }
}
