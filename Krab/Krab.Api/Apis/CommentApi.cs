using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Krab.Api.Constants;
using Krab.Api.ValueObjects.Comment;
using Krab.DataAccess.Dac;
using Newtonsoft.Json;

namespace Krab.Api.Apis
{
    public interface ICommentApi
    {
        Listing GetNewBySubreddit(int userId, string subreddit, int maxResults);
        Task<Listing> GetNewBySubredditAsync(int userId, string subreddit, int maxResults);
        
        void SubmitComment(int userId, string parentCommentId, string comment);
        Task SubmitCommentAsync(int userId, string parentCommentId, string comment);
    }
    
    public class CommentApi : ICommentApi
    {
        private readonly IRedditUserDac _redditUserDac;
        private readonly IAuthApi _authApi;

        public CommentApi(IRedditUserDac redditUserDac, IAuthApi authApi)
        {
            _redditUserDac = redditUserDac;
            _authApi = authApi;
        }

        public Listing GetNewBySubreddit(int userId, string subreddit, int maxResults)
        {
            Listing listing = null;

            Task.Run(async () => listing = await GetNewBySubredditAsync(userId, subreddit, maxResults)).Wait();

            return listing;
        }

        public async Task<Listing> GetNewBySubredditAsync(int userId, string subreddit, int maxResults)
        {
            Listing listing;

            var url = $"https://oauth.reddit.com/r/{subreddit}/comments.json?limit={maxResults}";

            var redditUser = _redditUserDac.GetByUser(userId)?.FirstOrDefault();

            if (redditUser == null)
                return null;

            var accessToken = await _authApi.GetAccessTokenAsync(redditUser.Id);

            using (var client = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            }))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
                client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);

                var responseMsg = await client.GetAsync(url);

                if (!responseMsg.IsSuccessStatusCode)
                    throw new HttpRequestException(responseMsg.ReasonPhrase);

                var json = await responseMsg.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                listing = JsonConvert.DeserializeObject<Listing>(json);
            }

            return listing;
        }

        public void SubmitComment(int userId, string parentCommentId, string comment)
        {
            Task.Run(async () => await SubmitCommentAsync(userId, parentCommentId, comment)).Wait();
        }

        public async Task SubmitCommentAsync(int userId, string parentCommentId, string comment)
        {
            var redditUser = _redditUserDac.GetByUser(userId)?.FirstOrDefault();

            if (redditUser == null)
                return;

            var accessToken = await _authApi.GetAccessTokenAsync(redditUser.Id);

            using (var client = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            }))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
                client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);

                var json = JsonConvert.SerializeObject(new SubmitRequest
                {
                    ParentId = parentCommentId,
                    Comment = comment
                });

                var response = await client.PostAsync(Urls.Comment, new StringContent(json));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
