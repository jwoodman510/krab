using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Krab.Api.Constants;
using Krab.Api.ValueObjects;
using Krab.Caching;
using Krab.DataAccess.Dac;
using Krab.DataAccess.User;
using Krab.Global;
using Newtonsoft.Json;
using RedditUser = Krab.Api.ValueObjects.RedditUser;

namespace Krab.Api.Apis
{
    public interface IAuthApi
    {
        Task SaveInitialTokens(string authorizationCode, string userId);

        string GetAccessToken(int redditUserId);

        Task<string> GetAccessTokenAsync(int redditUserId);
    }

    public class AuthApi : IAuthApi
    {
        private readonly IRedditUserDac _redditUserDac;
        private readonly IUserDac _userDac;
        private readonly ICache _cache;

        public AuthApi(IRedditUserDac redditUserDac, IUserDac userDac, ICache cache)
        {
            _redditUserDac = redditUserDac;
            _userDac = userDac;
            _cache = cache;
        }

        public async Task SaveInitialTokens(string authorizationCode, string userId)
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            }))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{AppSettings.ClientId}:{AppSettings.ClientSecret}");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(byteArray));

                var response = await client.PostAsync(Urls.GetAccessTokenUrl, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type", "authorization_code"),
                    new KeyValuePair<string,string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", AppSettings.RedirectUri)
                }));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(response.ReasonPhrase);

                var json = await response.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<Tokens>(json);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {tokens.AccessToken}");
                client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);

                response = await client.GetAsync(Urls.Me);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(response.ReasonPhrase);

                json = await response.Content.ReadAsStringAsync();
                var redditUser = JsonConvert.DeserializeObject<RedditUser>(json);

                if (string.IsNullOrEmpty(redditUser.Name))
                    return;

                var cacheKey = $"UserIds:{userId}";
                var cachedUser = _cache.GetValue<CachedUser>(cacheKey);

                var intUserId = cachedUser?.UserId ?? (_userDac.Get(userId)?.UserId ?? 0);

                if (cachedUser == null)
                {
                    _cache.SetValue(cacheKey, new CachedUser
                    {
                        Id = userId,
                        UserId = intUserId
                    }, 60 * 60);
                }

                var existing = _redditUserDac.GetByUser(intUserId)?.FirstOrDefault(u => u.UserName != redditUser.Name);

                if (existing == null)
                {
                    var newRedditUser = _redditUserDac.Create(new DataAccess.RedditUser.RedditUser
                    {
                        UserName = redditUser.Name,
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken,
                        UserId = intUserId
                    });

                    _cache.SetValue(CacheKeys.Tokens(newRedditUser.Id), tokens, tokens.ExpiresIn - 120);
                }
                else
                {
                    _redditUserDac.UpdateTokens(existing.Id, tokens.AccessToken, tokens.RefreshToken);

                    _cache.SetValue(CacheKeys.Tokens(existing.Id), tokens, tokens.ExpiresIn - 120);
                }
            }
        }

        public string GetAccessToken(int redditUserId)
        {
            string token = null;

            Task.Run(async () => token = await GetAccessTokenAsync(redditUserId)).Wait();

            return token;
        }

        public async Task<string> GetAccessTokenAsync(int redditUserId)
        {
            var tokens = _cache.GetValue<Tokens>(CacheKeys.Tokens(redditUserId));

            if (tokens != null)
                return tokens.AccessToken;

            var user = _redditUserDac.Get(redditUserId);

            if (user == null)
                return null;

            // get new access
            using (var client = new HttpClient(new HttpClientHandler()))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{AppSettings.ClientId}:{AppSettings.ClientSecret}");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("User-Agent", "My Reddit v1.0 by stagnant_waffle");

                var response = await client.PostAsync(Urls.GetAccessTokenUrl, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", user.RefreshToken)
                }));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(response.ReasonPhrase);

                var json = await response.Content.ReadAsStringAsync();
                tokens = JsonConvert.DeserializeObject<Tokens>(json);
            }

            _redditUserDac.UpdateAccessToken(redditUserId, tokens.AccessToken);

            _cache.SetValue(CacheKeys.Tokens(redditUserId), tokens, tokens.ExpiresIn - 120);

            return tokens.AccessToken;
        }
    }
}
