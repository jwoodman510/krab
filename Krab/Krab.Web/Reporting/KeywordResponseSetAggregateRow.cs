namespace Krab.Web.Reporting
{
    public class KeywordResponseSetAggregateRow : IReportRow
    {
        public int Id { get; set; }

        public string Keyword { get; set; }

        public string Response { get; set; }

        public long NumberOfResponses { get; set; }
    }
}