using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Krab.Web.Bootstrap;
using Krab.DataAccess.User;

namespace Krab.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
      
            UnityConfig.RegisterComponents();
            Bootstrapper.Configure(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


    }
}
