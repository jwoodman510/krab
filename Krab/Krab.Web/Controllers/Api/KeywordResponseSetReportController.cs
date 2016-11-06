using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.KeywordResponseSetSubredditReport;
using Krab.DataAccess.Subreddit;
using Krab.Web.Models.Response;
using Newtonsoft.Json;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetReportController : BaseController
    {
        public enum ReportType
        {
            Standard = 0,
            Subreddit = 1
        }

        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly IKeywordResponseSetSubredditReportDac _reportDac;
        private readonly ISubredditDac _subredditDac;

        public KeywordResponseSetReportController(
            IKeywordResponseSetDac keywordResponseSetDac,
            IKeywordResponseSetSubredditReportDac reportDac,
            ISubredditDac subredditDac)
        {
            _keywordResponseSetDac = keywordResponseSetDac;
            _reportDac = reportDac;
            _subredditDac = subredditDac;
        }

        [HttpGet]
        public OkResponse<IList<IReportRow>> Get(long startDateMs, long endDateMs, ReportType reportType)
        {
            var startDateUtc = new DateTime(1970,1,1,0,0,0,0).AddMilliseconds(startDateMs);
            var endDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(endDateMs);

            var krSets = _keywordResponseSetDac.GetByUserId(GetUserId())
                ?.ToDictionary(k => k.Id) ?? new Dictionary<int, KeywordResponseSet>();

            var reportData = _reportDac.GetByKeywordResponseSetsDateRange(
                krSets.Select(s => s.Key),
                startDateUtc,
                endDateUtc);

            if (reportType == ReportType.Subreddit)
            {
                var toReturn = GetSubredditReport(reportData, krSets);

                return new OkResponse<IList<IReportRow>>(toReturn);
            }
            else
            {
                var toReturn = GetStandardReport(reportData, krSets);

                return new OkResponse<IList<IReportRow>>(toReturn);
            }
        }

        private static List<IReportRow> GetStandardReport(IQueryable<KeywordResponseSetSubredditReport> reportData, IReadOnlyDictionary<int, KeywordResponseSet> krSets)
        {
            var rows =
                from d in reportData
                group d by new
                {
                    d.ReportDateUtc,
                    d.KeywordResponseSetId
                }
                into grp
                select new KeywordResponseSetRow
                {
                    Id = grp.Key.KeywordResponseSetId,
                    ReportDateUtc = grp.Key.ReportDateUtc,
                    NumberOfResponses = grp.Sum(g => g.NumResponses)
                };

            var toReturn = rows
                .OrderBy(r => r.ReportDateUtc)
                .ThenBy(r => r.Id)
                .ToList();

            toReturn.ForEach(r =>
            {
                r.Keyword = krSets[r.Id].Keyword;
                r.Response = krSets[r.Id].Response;
            });

            return new List<IReportRow>(toReturn);
        }
        
        private List<IReportRow> GetSubredditReport(IQueryable<KeywordResponseSetSubredditReport> reportData, IReadOnlyDictionary<int, KeywordResponseSet> krSets)
        {
            var subreddits = _subredditDac.GetByKeywordResponseSetIds(krSets.Select(s => s.Key))
                                 ?.ToDictionary(s => (long)s.Id) ?? new Dictionary<long, Subreddit>();

            var rows =
                from d in reportData
                group d by new
                {
                    d.ReportDateUtc,
                    d.KeywordResponseSetId,
                    d.SubredditId
                }
                into grp
                select new KeywordResponseSetSubredditRow
                {
                    Id = grp.Key.KeywordResponseSetId,
                    SubredditId = grp.Key.SubredditId,
                    ReportDateUtc = grp.Key.ReportDateUtc,
                    NumberOfResponses = grp.Sum(g => g.NumResponses)
                };

            var toReturn = rows
                .OrderBy(r => r.ReportDateUtc)
                .ThenBy(r => r.Id)
                .ThenBy(t => t.SubredditId)
                .ToList();

            toReturn = toReturn.Where(r => subreddits.ContainsKey(r.SubredditId)).ToList();

            toReturn.ForEach(r =>
            {
                r.Keyword = krSets[r.Id].Keyword;
                r.Response = krSets[r.Id].Response;
                r.Subreddit = subreddits[r.SubredditId].SubredditName;
            });

            return new List<IReportRow>(toReturn);
        }

        public interface IReportRow
        {

        }

        public class KeywordResponseSetRow : IReportRow
        {
            public int Id { get; set; }

            public DateTime ReportDateUtc { get; set; }

            public string Keyword { get; set; }

            public string Response { get; set; }

            public long NumberOfResponses { get; set; }
        }
        
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
}