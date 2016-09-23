namespace Krab.Global
{
    public static class CacheKeys
    {
        public static string RedditAuthState(string userId) => $"RedditAuth:{userId}";
        public static string Tokens(int id) => $"RedditTokens:{id}";
    }
}
