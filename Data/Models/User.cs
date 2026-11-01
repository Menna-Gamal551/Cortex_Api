using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }

        public Device? device { get; set; }
        public ICollection<AccountMetaData>? AccountMetaDatas { get; set; }
        public ICollection<Auditlog>?Auditlogs { get; set; }
        public ICollection<SecurityEvent>?SecurityEvents { get; set; }

    }
}
