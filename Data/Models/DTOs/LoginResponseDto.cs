namespace Cortex_Api.Data.Models.DTOs
{
    public class LoginResponseDto
    {
       public string? Token { get; set; }
        public UserDto? userDTO { get; set; }
    }
}
