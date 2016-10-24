using Microsoft.Practices.Unity;
using StackExchange.Redis;

namespace Krab.Caching
{
    public static class Configuration
    {
        public static void RegisterInMemoryCache(IUnityContainer container)
        {
            container.RegisterType<ICache, InMemoryCache>();
        }
        public static void RegisterRedisCache(IUnityContainer container, string connectionString = null)
        {
            container.RegisterInstance<ICache>(new RedisCache(ConnectionMultiplexer.Connect(connectionString ?? "127.0.0.1")));
        }
    }
}
