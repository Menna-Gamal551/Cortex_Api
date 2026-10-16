using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        [MaxLength(100)]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
