using System;
using System.Runtime.Caching;
using Newtonsoft.Json;

namespace Krab.Caching
{
    public class Cache : ICache
    {
        private static MemoryCache MemoryCache => MemoryCache.Default;

        public void SetValue(string key, object value, DateTimeOffset expiration)
        {
            var json = JsonConvert.SerializeObject(value);
            MemoryCache.Set(key, json, expiration);
        }

        public void SetValue(string key, object value, long secondsToKeep)
        {
            if (secondsToKeep <= 0)
                throw new InvalidOperationException("Expiration must be greater than 0.");

            var expiration = DateTimeOffset.UtcNow.AddSeconds(secondsToKeep);
            var json = JsonConvert.SerializeObject(value);
            MemoryCache.Set(key, json, expiration);
        }

        public T GetValue<T>(string key)
        {
            var value = MemoryCache.Get(key);

            return value == null
                ? default(T)
                : JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public void Delete(string key)
        {
            if (MemoryCache.Contains(key))
                MemoryCache.Remove(key);
        }

        public void Dispose()
        {
            MemoryCache.Dispose();
        }
    }
}
