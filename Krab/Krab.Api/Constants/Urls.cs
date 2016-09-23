namespace Krab.Api.Constants
{
    public static class Urls
    {
        public static string GetAccessTokenUrl = "https://www.reddit.com/api/v1/access_token";

        public static string ApiUrl = "https://oauth.reddit.com";

        public static string Me => ApiUrl + "/api/v1/me";
    }
}
