using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class BreachResult
    {
        [Key]
        public int Id { get; set; }
 
        public DateTime CheckedAT { get; set; }
        [MaxLength(30)]
        public string Status { get; set; }
        [MaxLength(200)]
        public string? Source { get; set; }
        [MaxLength(1000)]
        public string? Details { get; set; }
        [ForeignKey("AccountMetaDataId")]
        public int AccountMetaDataId { get; set; }
        public AccountMetaData? AccountMetaDatas { get; set; }
    }
}
