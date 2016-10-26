using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using Krab.Global;
using Krab.Mail;
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

            var providers = config.Services.GetFilterProviders().ToList();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);

            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
        }

        public static void RegisterInstances(IUnityContainer container)
        {
            container.RegisterInstance(typeof (IMailClient), new SendGridMailClient(AppSettings.SendGridApiKey));

            DataAccess.Configuration.Register(container);
            Caching.Configuration.RegisterInMemoryCache(container);
            Api.Configuration.Register(container);
        }
    }
}