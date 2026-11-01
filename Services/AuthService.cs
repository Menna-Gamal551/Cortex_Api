
using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cortex_Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISecurityEventService _securityEventService;

        public AuthService(
            AppDbContext db,
            IConfiguration configuration,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ISecurityEventService securityEventService)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _securityEventService = securityEventService;
        }

        public async Task<bool> IsUserNameExistAsync(string userName)
        {
            userName = userName.Trim();

            return await _db.Users
                .AnyAsync(u => u.UserName == userName);
        }

        public async Task<UserDto> RegisterAsync(
            RegisterRequestDto registerRequestDto)
        {
            ArgumentNullException.ThrowIfNull(registerRequestDto);

            var userName = registerRequestDto.UserName.Trim();

            if (await IsUserNameExistAsync(userName))
            {
                throw new InvalidOperationException(
                    "Username already exists.");
            }

            var user = new User
            {
                Name = registerRequestDto.Name.Trim(),
                UserName = userName,
                PasswordHash = string.Empty,
                CreatedDate = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                registerRequestDto.Password);

            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                    && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new InvalidOperationException(
                        "Username already exists.", ex);
                }

                throw;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto loginRequestDto)
        {
            var userName = loginRequestDto.UserName.Trim();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                await _securityEventService.LogAsync(
                    SecurityEventSeverity.Medium,
                    "FailedLogin",
                    "Login failed: invalid credentials.");

                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginRequestDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                await _securityEventService.LogAsync(
                    SecurityEventSeverity.Medium,
                    "FailedLogin",
                    "Login failed: invalid credentials.",
                    userId: user.Id);

                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            // Update the password hash if the hasher recommends it.
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(
                    user,
                    loginRequestDto.Password);

                user.UpdateDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                userDTO = _mapper.Map<UserDto>(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var secret = _configuration["jwtsettings:secret"];

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException(
                    "JWT secret is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(secret);

            if (key.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT secret must be at least 32 bytes.");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        user.Name),

                    new Claim(
                        ClaimTypes.NameIdentifier + "_username",
                        user.UserName)
                }),

                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
