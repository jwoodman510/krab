using Microsoft.Practices.Unity;

namespace Krab.Caching
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<ICache, Cache>();
        }
    }
}
