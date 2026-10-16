using Cortex_Api.Data.Models.DTOs;

namespace Cortex_Api.Services.IServices
{
    public interface IAuthService
    {
        Task<bool> IsUserNameExistAsync(string userName);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequestDto);

    }
}
