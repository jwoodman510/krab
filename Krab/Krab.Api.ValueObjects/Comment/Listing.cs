using Newtonsoft.Json;

namespace Krab.Api.ValueObjects.Comment
{
    public class Listing
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
