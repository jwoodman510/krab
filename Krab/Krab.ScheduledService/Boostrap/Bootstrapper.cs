using log4net;
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
            container.RegisterInstance(typeof(ILog), LogManager.GetLogger("ServiceLogger"));

            Configuration.Register(container);
            DataAccess.Configuration.Register(container);
            Caching.Configuration.Register(container);
            Api.Configuration.Register(container);
            Global.Configuration.Register(container);
        }
    }
}
