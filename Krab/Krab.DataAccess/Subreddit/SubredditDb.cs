namespace Krab.DataAccess.Subreddit
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SubredditDb : DbContext
    {
        public SubredditDb()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Subreddit> Subreddits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
