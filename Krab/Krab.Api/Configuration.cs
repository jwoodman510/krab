using Krab.Api.Apis;
using Microsoft.Practices.Unity;

namespace Krab.Api
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IAuthApi, AuthApi>();
        }
    }
}
