namespace Krab.DataAccess.Subreddit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Subreddit
    {
        public int Id { get; set; }

        public int KeywordResponseSetId { get; set; }

        [Column("Subreddit")]
        [Required]
        [StringLength(1028)]
        public string SubredditName { get; set; }
    }
}
