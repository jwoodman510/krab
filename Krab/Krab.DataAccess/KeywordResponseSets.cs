namespace Krab.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class KeywordResponseSetsDb : DbContext
    {
        public KeywordResponseSetsDb()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<KeywordResponseSet> KeywordResponseSets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
