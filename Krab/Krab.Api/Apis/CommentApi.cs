using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Krab.Api.ValueObjects.Comment;
using Newtonsoft.Json;

namespace Krab.Api.Apis
{
    public interface ICommentApi
    {
        IList<Comment> GetNewBySubreddit(string subreddit, int maxResults);
        Task<IList<Comment>> GetNewBySubredditAsync(string subreddit, int maxResults);
    }

    public class CommentApi : ICommentApi
    {
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
    }
}
