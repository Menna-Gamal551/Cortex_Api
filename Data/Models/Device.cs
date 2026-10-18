using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string  DeviceUid  { get; set; }
        [MaxLength(50)]
        public string? FirmwareVersion { get; set; }
        public DateTime PairedAt { get; set; }
        public DateTime? LastConnectedAt { get; set;}
        public bool IsPaired { get; set;}
        public ICollection<Auditlog>? Auditlogs { get; set; }
        public ICollection<SecurityEvent>? SecurityEvents { get; set; }
    }
}
