using Newtonsoft.Json;

namespace Krab.Api.ValueObjects.Comment
{
    public class CommentJson
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("data")]
        public Comment Comment { get; set; }
    }

    public class Comment
    {
        // ParentId == Parent.Name

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("subreddit_id")]
        public string SubredditId { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("ups")]
        public long Ups { get; set; }

        [JsonProperty("downs")]
        public long Downs { get; set; }
        
        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("over_18")]
        public long IsOverEighteen { get; set; }
    }
}