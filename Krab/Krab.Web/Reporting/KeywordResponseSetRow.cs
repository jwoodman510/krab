using System;

namespace Krab.Web.Reporting
{
    public class KeywordResponseSetRow : IReportRow
    {
        public int Id { get; set; }

        public DateTime ReportDateUtc { get; set; }

        public string Keyword { get; set; }

        public string Response { get; set; }

        public long NumberOfResponses { get; set; }
    }
}