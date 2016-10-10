using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Krab.Web.Startup))]

namespace Krab.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
