namespace Krab.DataAccess.KeywordResponseSetSubredditReport
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class KeywordResponseSetSubredditReportDb : DbContext
    {
        public KeywordResponseSetSubredditReportDb()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<KeywordResponseSetSubredditReport> KeywordResponseSetSubredditReports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
