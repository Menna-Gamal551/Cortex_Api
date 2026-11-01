
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class AccountMetaData
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string? UserName { get; set; }

        [EmailAddress]
        [MaxLength(254)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? WebsiteURL { get; set; }

        [Required]
        [MaxLength(150)]
        public required string ServiceName { get; set; }

        public bool HasTOTP { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<BreachResult> BreachResults { get; set; }
            = new List<BreachResult>();
    }
}

