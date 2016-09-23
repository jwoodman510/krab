using System.Configuration;

namespace Krab.Global
{
    public static class AppSettings
    {
#if DEBUG
        public static string RedirectUri => "https://localhost:44300/Home/AuthorizationCallback";
        public static string ClientId => "8lO9pAWfDK0Aqg";
        public static string ClientSecret => "lPwavynJWkVU03kN2oSh8CoEnz8";
#else
            public static string RedirectUri => "https://krab.azurewebsites.net/Home/AuthorizationCallback";
            public static string ClientId => "7c9AhacF0DtugQ";
            public static string ClientSecret => "WX9dvCbFo0ViKJx5IOS2r15d35o";
#endif
    }
}
