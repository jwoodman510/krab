using System.Web.Http;
using Krab.Web.Controllers;
using Microsoft.Practices.Unity;

namespace Krab.Web.Bootstrap
{
    public static class Bootstrapper
    {
        public static void Configure(HttpConfiguration config)
        {
            var container = new UnityContainer();
            
            RegisterInstances(container);

            config.DependencyResolver = new UnityResolver(container);
        }

        public static void RegisterInstances(IUnityContainer container)
        {
            DataAccess.Configuration.Register(container);
            Cache.Configuration.Register(container);
            RegisterMvcControllers(container);
        }

        private static void RegisterMvcControllers(IUnityContainer container)
        {
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());
        }
    }
}