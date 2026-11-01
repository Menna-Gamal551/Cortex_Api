
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
        public required string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public int? DeviceId { get; set; }

        [ForeignKey(nameof(DeviceId))]
        public Device? Device { get; set; }

        public ICollection<AccountMetaData> AccountMetaDatas { get; set; }
            = new List<AccountMetaData>();

        public ICollection<Auditlog> Auditlogs { get; set; }
            = new List<Auditlog>();

        public ICollection<SecurityEvent> SecurityEvents { get; set; }
            = new List<SecurityEvent>();
    }
}

