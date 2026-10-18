using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class SecurityEvent
    {
        [Key]
        public int Id { get; set; }
       
        [MaxLength(20)]
        public string Severity { get; set; }
        [MaxLength(100)]
        public string EventType { get; set; }
        public DateTime OccuredAt { get; set; } = DateTime.Now;
        [MaxLength(1000)]
        public string? Details { get; set; }
        [ForeignKey("UserId")]
        public int? UserId { get; set; }
        public User? user { get; set; }
        [ForeignKey("DeviceId")]
        public int? DeviceId { get; set; }
        public Device? device { get; set; }
    
}
}
