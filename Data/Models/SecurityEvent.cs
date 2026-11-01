
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class SecurityEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public SecurityEventSeverity Severity { get; set; }

        [Required]
        [MaxLength(100)]
        public required string EventType { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Details { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int? DeviceId { get; set; }

        [ForeignKey(nameof(DeviceId))]
        public Device? Device { get; set; }
    }
}

