using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class Auditlog
    {
        [Key]
        public int Id { get; set; }
       
        [MaxLength(100)]
        public required string Action { get; set; }
        public DateTime OccuredAt { get; set; }= DateTime.UtcNow;
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
