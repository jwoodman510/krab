namespace Krab.DataAccess.RedditUser
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RedditUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
