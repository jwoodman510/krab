using System;
using System.Collections.Generic;
using System.Web.Http;
using Krab.Web.Models.Response;
using Krab.Web.Reporting;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetReportController : BaseController
    {
        private readonly IReportService _reportService;

        public KeywordResponseSetReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public OkResponse<IList<IReportRow>> Get(long startDateMs, long endDateMs, ReportType reportType)
        {
            var startDateUtc = new DateTime(1970,1,1,0,0,0,0).AddMilliseconds(startDateMs);
            var endDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(endDateMs);
            var userId = GetUserId();

            switch (reportType)
            {
                case ReportType.Subreddit:
                    var sRows = _reportService.GetSubredditReport(startDateUtc, endDateUtc, userId);
                    return new OkResponse<IList<IReportRow>>(sRows);

                case ReportType.StandardAggregate:
                    var aRows = _reportService.GetStandardAggregateReport(startDateUtc, endDateUtc, userId);
                    return new OkResponse<IList<IReportRow>>(aRows);
            }
            
            var rptRows = _reportService.GetStandardReport(startDateUtc, endDateUtc, userId);
            return new OkResponse<IList<IReportRow>>(rptRows);
        }
    }
}