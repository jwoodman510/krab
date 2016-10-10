using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Newtonsoft.Json.Serialization;

namespace Krab.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "POSTs",
                routeTemplate: "api/v1/{controller}/{action}",
                defaults: new { },
                constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("POST")) }
                );

            config.Routes.MapHttpRoute(
                name: "PUTs",
                routeTemplate: "api/v1/{controller}/{action}",
                defaults: new { },
                constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("PUT")) }
                );

            config.Routes.MapHttpRoute(
                name: "DELETEs",
                routeTemplate: "api/v1/{controller}/{action}",
                defaults: new { },
                constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("DELETE")) }
                );
        }
    }
}
