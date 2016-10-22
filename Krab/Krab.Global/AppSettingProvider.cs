using System;
using System.Configuration;

namespace Krab.Global
{
    public interface IAppSettingProvider
    {
        int GetInt(string key);

        bool GetBool(string key);
    }

    public class AppSettingProvider : IAppSettingProvider
    {
        public int GetInt(string key)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings[key]);
        }

        public bool GetBool(string key)
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings[key]);
        }
    }
}
