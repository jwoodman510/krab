using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Krab.Caching
{
    public class RedisCache : ICache
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        
        public RedisCache(ConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public void SetValue(string key, object value, long secondsToKeep)
        {
            var json = JsonConvert.SerializeObject(value);
            var db = _connectionMultiplexer.GetDatabase();

            db.StringSet(key, json);
            db.KeyExpire(key, DateTime.Now.AddSeconds(secondsToKeep));
        }

        public T GetValue<T>(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var value = db.StringGet(key);

            if (value.IsNull)
                return default(T);

            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public void Delete(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            db.KeyDelete(key);
        }

        public void Dispose()
        {
            _connectionMultiplexer?.Dispose();
        }
    }
}
