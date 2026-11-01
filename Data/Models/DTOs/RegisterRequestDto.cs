using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public required string UserName { get; set; }
        [Required]
        [MinLength(8)]
        public required string Password { get; set; }
    }
}
