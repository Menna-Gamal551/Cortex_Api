using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cortex_Api.Services
{
    public class AuthService:IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService( AppDbContext db, IConfiguration configuration,IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _db = db;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> IsUserNameExistAsync(string userName)
        {
            return await _db.Users.AnyAsync(e=>e.UserName==userName);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(e=>e.UserName==loginRequestDto.UserName);
            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                loginRequestDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }
            var token=GeneratejwtToken(user);
            return new LoginResponseDto()
            {
                Token = token,
                userDTO=_mapper.Map<UserDto>(user)
            };
        }
        public async Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            if (registerRequestDto == null)
            {
                throw new ArgumentNullException( nameof(registerRequestDto));
            }

            if (await IsUserNameExistAsync(registerRequestDto.UserName))
            {
                throw new InvalidOperationException( "Username already exists.");
            }

            User user = new()
            {
                Name = registerRequestDto.Name,
                UserName = registerRequestDto.UserName,
                CreatedDate = DateTime.Now,
                Password = string.Empty
            };

            user.Password = _passwordHasher.HashPassword(
                user,
                registerRequestDto.Password);

            await _db.Users.AddAsync(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                    && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new InvalidOperationException( "Username already exists.", ex);
                }

                throw;
            }

            return _mapper.Map<UserDto>(user);
        }
       private string GeneratejwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("jwtsettings")["secret"]);
            var tokenDescriptor=new SecurityTokenDescriptor 
            {
                Subject=new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Name),

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    
    }
}
