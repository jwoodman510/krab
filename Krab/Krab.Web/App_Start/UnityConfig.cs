using System.Web.Mvc;
using Krab.Web.Bootstrap;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace Krab.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            Bootstrapper.RegisterInstances(container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}