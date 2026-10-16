using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class UserDto
    {
        
//public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        //[Required]
        //[MaxLength(100)]
        //public required string UserName { get; set; }
    }
}
