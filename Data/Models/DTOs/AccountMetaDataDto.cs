
using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class AccountMetaDataDto
    {
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

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

