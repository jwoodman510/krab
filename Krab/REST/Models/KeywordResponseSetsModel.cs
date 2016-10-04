using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace REST.Models
{
    public partial class KeywordResponseSetsModel
     {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string Keyword { get; set; }

        [Required]
        [StringLength(1000)]
        public string Response { get; set; }
    }
}