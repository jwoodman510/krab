using System.Collections.Generic;
using Newtonsoft.Json;

namespace Krab.Api.ValueObjects.Comment
{
    public class Data
    {
        [JsonProperty("modhash")]
        public string ModHash { get; set; }

        [JsonProperty("children")]
        public List<CommentJson> Children { get; set; }
    }
}
