using System.Data.Entity;

namespace Krab.DataAccess.KeywordResponseSet
{
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
