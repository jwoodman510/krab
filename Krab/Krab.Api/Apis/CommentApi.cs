using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Krab.Api.Constants;
using Krab.Api.ValueObjects.Comment;
using Krab.DataAccess.Dac;
using Newtonsoft.Json;

namespace Krab.Api.Apis
{
    public interface ICommentApi
    {
        IList<Comment> GetNewBySubreddit(string subreddit, int maxResults);
        Task<IList<Comment>> GetNewBySubredditAsync(string subreddit, int maxResults);

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

        public IList<Comment> GetNewBySubreddit(string subreddit, int maxResults)
        {
            IList<Comment> comments = new List<Comment>();

            Task.Run(async () => comments = await GetNewBySubredditAsync(subreddit, maxResults)).Wait();

            return comments;
        }

        public async Task<IList<Comment>> GetNewBySubredditAsync(string subreddit, int maxResults)
        {
            var comments = new List<Comment>();

            var url = $"https://www.reddit.com/r/{subreddit}/comments.json?limit={maxResults}";

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                var responseMsg = await client.GetAsync(url);

                var json = await responseMsg.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return comments;

                var listing = JsonConvert.DeserializeObject<Listing>(json);

                comments = listing?.Data?.Children?.Select(c => c.Comment).ToList() ?? comments;
            }

            return comments;
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

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
                client.DefaultRequestHeaders.Add("User-Agent", Settings.UserAgent);

                var json = JsonConvert.SerializeObject(new SubmitRequest
                {
                    ParentId = parentCommentId,
                    Comment = comment
                });

                var response = await client.PostAsync(Urls.Comment, new StringContent(json));//, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
