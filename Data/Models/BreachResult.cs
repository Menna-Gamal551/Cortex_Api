
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class BreachResult
    {
        [Key]
        public int Id { get; set; }

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public required string Status { get; set; }

        [MaxLength(200)]
        public string? Source { get; set; }

        [MaxLength(1000)]
        public string? Details { get; set; }

        public int AccountMetaDataId { get; set; }

        [ForeignKey(nameof(AccountMetaDataId))]
        public AccountMetaData? AccountMetaData { get; set; }
    }
}
