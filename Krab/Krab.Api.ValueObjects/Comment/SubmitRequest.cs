using Newtonsoft.Json;

namespace Krab.Api.ValueObjects.Comment
{
    public class SubmitRequest
    {
        [JsonProperty("thing_id")]
        public string ParentId { get; set; }

        [JsonProperty("text")]
        public string Comment { get; set; }

        [JsonProperty("uh")]
        public string ModHash { get; set; }
        
        [JsonProperty("api_type")]
        public string ApiType => "json";
    }
}
