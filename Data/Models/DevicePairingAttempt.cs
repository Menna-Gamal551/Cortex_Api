
using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models
{
    public class DevicePairingAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string PairingId { get; set; }

        [Required]
        public required string Challenge { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }
    }
}
