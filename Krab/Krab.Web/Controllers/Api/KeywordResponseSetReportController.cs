using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.KeywordResponseSetSubredditReport;
using Krab.Web.Models.Response;
using Newtonsoft.Json;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetReportController : BaseController
    {
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly IKeywordResponseSetSubredditReportDac _reportDac;

        public KeywordResponseSetReportController(IKeywordResponseSetDac keywordResponseSetDac, IKeywordResponseSetSubredditReportDac reportDac)
        {
            _keywordResponseSetDac = keywordResponseSetDac;
            _reportDac = reportDac;
        }

        [HttpGet]
        public OkResponse<IList<KeywordResponseSetRow>> Get(long startDateMs, long endDateMs)
        {
            var startDateUtc = new DateTime(1970,1,1,0,0,0,0).AddMilliseconds(startDateMs);
            var endDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(endDateMs);

            var krSets = _keywordResponseSetDac.GetByUserId(GetUserId())
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

            var toReturn = rows.OrderBy(r => r.ReportDateUtc).ToList();
            toReturn.ForEach(r =>
            {
                r.Keyword = krSets[r.Id].Keyword;
                r.Response = krSets[r.Id].Response;
            });

            return new OkResponse<IList<KeywordResponseSetRow>>(toReturn);
        }

        public class KeywordResponseSetRow
        {
            public int Id { get; set; }

            public DateTime ReportDateUtc { get; set; }

            public string Keyword { get; set; }

            public string Response { get; set; }

            public long NumberOfResponses { get; set; }
        }
    }
}