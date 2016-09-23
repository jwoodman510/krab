using System;

namespace Krab.Caching
{
    public interface ICache
    {
        void SetValue(string key, object value, DateTimeOffset expiration);

        void SetValue(string key, object value, long secondsToKeep);

        T GetValue<T>(string key);

        void Delete(string key);

        void Dispose();
    }
}
