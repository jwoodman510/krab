using Newtonsoft.Json;

namespace Krab.Api.ValueObjects
{
    public class RedditUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("over_18")]
        public bool IsOverEighteen { get; set; }
    }
}