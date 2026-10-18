using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortex_Api.Data.Models
{
    public class AccountMetaData
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        public string ?UserName { get; set; }
        [EmailAddress]
        [MaxLength(254)]
        public string? Email { get; set; }   
        
        [MaxLength(500)]
        public string? WebsiteURL { get; set; }
        [MaxLength(150)]
        public string ServiceName { get; set; }
        public bool HasTOTP { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime? UpdatedAt { get; set;}
        [ForeignKey("UserId")]
        public  int UserId { get; set; }
        public User? user { get; set; }
        public ICollection<BreachResult>? BreachResults { get; set; }

    }
}
