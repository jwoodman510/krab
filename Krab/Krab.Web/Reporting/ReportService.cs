using System;
using System.Collections.Generic;
using System.Linq;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.Subreddit;

namespace Krab.Web.Reporting
{
    public interface IReportService
    {
        IList<IReportRow> GetStandardReport(DateTime startDateUtc, DateTime endDateUtc, int userId);
        IList<IReportRow> GetSubredditReport(DateTime startDateUtc, DateTime endDateUtc, int userId);
        IList<IReportRow> GetStandardAggregateReport(DateTime startDateUtc, DateTime endDateUtc, int userId);
    }

    public class ReportService : IReportService
    {
        private readonly ISubredditDac _subredditDac;
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly IKeywordResponseSetSubredditReportDac _reportDac;

        public ReportService(ISubredditDac subredditDac, IKeywordResponseSetDac keywordResponseSetDac, IKeywordResponseSetSubredditReportDac reportDac)
        {
            _subredditDac = subredditDac;
            _keywordResponseSetDac = keywordResponseSetDac;
            _reportDac = reportDac;
        }

        public IList<IReportRow> GetStandardReport(DateTime startDateUtc, DateTime endDateUtc, int userId)
        {
            var krSets = _keywordResponseSetDac.GetByUserId(userId)
                   ?.ToDictionary(k => k.Id) ?? new Dictionary<int, KeywordResponseSet>();

            var reportData = _reportDac.GetByKeywordResponseSetsDateRange(
                krSets.Select(s => s.Key),
                startDateUtc,
                endDateUtc);

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

        public IList<IReportRow> GetSubredditReport(DateTime startDateUtc, DateTime endDateUtc, int userId)
        {
            var krSets = _keywordResponseSetDac.GetByUserId(userId)
                   ?.ToDictionary(k => k.Id) ?? new Dictionary<int, KeywordResponseSet>();

            var reportData = _reportDac.GetByKeywordResponseSetsDateRange(
                krSets.Select(s => s.Key),
                startDateUtc,
                endDateUtc);

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

        public IList<IReportRow> GetStandardAggregateReport(DateTime startDateUtc, DateTime endDateUtc, int userId)
        {
            var krSets = _keywordResponseSetDac.GetByUserId(userId)
                   ?.ToDictionary(k => k.Id) ?? new Dictionary<int, KeywordResponseSet>();

            var reportData = _reportDac.GetByKeywordResponseSetsDateRange(
                krSets.Select(s => s.Key),
                startDateUtc,
                endDateUtc);

            var rows =
                from d in reportData
                group d by new
                {
                    d.KeywordResponseSetId
                }
                into grp
                select new KeywordResponseSetAggregateRow
                {
                    Id = grp.Key.KeywordResponseSetId,
                    NumberOfResponses = grp.Sum(g => g.NumResponses)
                };

            var toReturn = rows
                .OrderBy(r => r.Id)
                .ToList();

            toReturn.ForEach(r =>
            {
                r.Keyword = krSets[r.Id].Keyword;
                r.Response = krSets[r.Id].Response;
            });

            return new List<IReportRow>(toReturn);
        }
    }
}