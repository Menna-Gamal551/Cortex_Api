using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cortex_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService= authService;
        [HttpGet("TestAuth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            return Ok(new
            {
                Message = "You are authenticated!",
                UserName = User.Identity?.Name
            });
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<ApiResponse<UserDto>>> Register(RegisterRequestDto registerRequestDTO)
        {
            try
            {
                if (registerRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration data is required."));
                }

                if (await _authService.IsUserNameExistAsync(registerRequestDTO.UserName))
                {
                    return Conflict(ApiResponse<object>.Conflict($"Username '{registerRequestDTO.UserName}' already exists."));
                }

                var userDTO = await _authService.RegisterAsync(registerRequestDTO);

                if (userDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration failed."));
                }

                var response = ApiResponse<UserDto>.CreatedAt("User registered successfully",userDTO);

                return StatusCode(StatusCodes.Status201Created,response);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Username already exists.")
                {
                    return Conflict(ApiResponse<object>.Conflict("Username already exists."));
                }

                return StatusCode(
                    StatusCodes.Status500InternalServerError,ApiResponse<object>.Error(500,"An unexpected error occurred.",null));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ApiResponse<object>.Error(500,"An unexpected error occurred.",null));
            }
        }


    
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Data is Required"));
                }


                var loginResponse = await _authService.LoginAsync(loginRequestDTO);
                if (loginResponse==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Failed"));
                }


                var response = ApiResponse<LoginResponseDto>.Ok("User Login Successfully", loginResponse);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<object>.Error( 401, "Invalid username or password.",null));
            }
            catch (Exception ex)
            {

                var errorresponse = ApiResponse<object>.Error( 500, "An unexpected error occurred.",  null);

                return StatusCode(500, errorresponse);
            }


        }

    }
}
