using System;
using System.ComponentModel.DataAnnotations;

namespace Krab.DataAccess.KeywordResponseSet
{
    public partial class KeywordResponseSet
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string Keyword { get; set; }

        [Required]
        [StringLength(1000)]
        public string Response { get; set; }

        public string Status
        {
            get
            {
                KeywordResponseSetStatus status;

                if (Enum.TryParse(StatusId.ToString(), out status))
                    return status.ToString();

                return KeywordResponseSetStatus.Unknown.ToString();
            }
        }
    }
}
