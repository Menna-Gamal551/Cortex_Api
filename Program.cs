using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Cortex_Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Cortex_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("jwtsettings")["secret"]);

            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                op.DefaultForbidScheme=JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata=false;
                op.SaveToken=true;
                op.TokenValidationParameters=new TokenValidationParameters
                {
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(key),
                    ValidateIssuer=false,
                    ValidateAudience=false,
                    ValidateLifetime=true,
                    ClockSkew=TimeSpan.Zero
                };
            }
            );
            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>
                 (option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyCon")));
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(op =>
            {
                op.CreateMap<User,UserDto >().ReverseMap();
                op.CreateMap<CreateAccountMetaDataDto,AccountMetaData >().ReverseMap();
                op.CreateMap<CreateAccountMetaDataDto,AccountMetaDataDto >().ReverseMap();
                op.CreateMap<UpdateAccountMetaDataDto,AccountMetaDataDto >().ReverseMap();
                op.CreateMap<UpdateAccountMetaDataDto,AccountMetaData >().ReverseMap();
                op.CreateMap<AccountMetaDataDto,AccountMetaData >().ReverseMap();
                op.CreateMap<Auditlog, AuditlogDto>().ReverseMap();
                op.CreateMap<SecurityEvent, SecurityEventDto>().ReverseMap();
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<ISecurityEventService, SecurityEventService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped< IPasswordHasher<User>, PasswordHasher<User>>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
