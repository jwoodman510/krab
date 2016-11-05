namespace Krab.DataAccess.KeywordResponseSetSubredditReport
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class KeywordResponseSetSubredditReportDb : DbContext
    {
        public KeywordResponseSetSubredditReportDb()
            : base("name=KeywordResponseSetSubredditReportDb")
        {
        }

        public virtual DbSet<KeywordResponseSetSubredditReport> KeywordResponseSetSubredditReports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
