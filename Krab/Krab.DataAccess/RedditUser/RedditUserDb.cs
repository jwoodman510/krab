namespace Krab.DataAccess.RedditUser
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RedditUserDb : DbContext
    {
        public RedditUserDb()
            : base("name=RedditUserDb")
        {
        }

        public virtual DbSet<RedditUser> RedditUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
