using System;
using Newtonsoft.Json;

namespace Krab.Web.Reporting
{
    public class KeywordResponseSetSubredditRow : IReportRow
    {
        public int Id { get; set; }

        [JsonIgnore]
        public long SubredditId { get; set; }

        public DateTime ReportDateUtc { get; set; }

        public string Keyword { get; set; }

        public string Response { get; set; }

        public string Subreddit { get; set; }

        public long NumberOfResponses { get; set; }
    }
}