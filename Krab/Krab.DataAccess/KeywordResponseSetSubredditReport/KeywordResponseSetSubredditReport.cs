namespace Krab.DataAccess.KeywordResponseSetSubredditReport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KeywordResponseSetSubredditReport
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KeywordResponseSetId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SubredditId { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime ReportDateUtc { get; set; }

        public long NumResponses { get; set; }

        public bool IsClosed { get; set; }
    }
}
