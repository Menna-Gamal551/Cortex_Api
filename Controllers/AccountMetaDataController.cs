
using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cortex_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMetaDataController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        public AccountMetaDataController(AppDbContext db, IMapper mapper, IAuditLogService auditLogService)
        {
            _db = db;
            _mapper = mapper;
            _auditLogService=auditLogService;
        }


        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AccountMetaDataDto>>>> GetAllAccountMetaData()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var accounts = await _db.AccountMetaDatas
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var accountsDto = _mapper.Map<IEnumerable<AccountMetaDataDto>>(accounts);

            return Ok(ApiResponse<IEnumerable<AccountMetaDataDto>>.Ok("Accounts retrieved successfully.", accountsDto));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AccountMetaDataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AccountMetaDataDto>>> GetAccountMetaDataById(int id)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.BadRequest( "AccountMetaData Id must be greater than 0."));
                }

                var accountMetaData = await _db.AccountMetaDatas.SingleOrDefaultAsync(e =>e.Id == id && e.UserId == userId);

                if (accountMetaData == null)
                {
                    return NotFound( ApiResponse<object>.NotFound( $"AccountMetaData with id {id} was not found."));
                }

                var dto = _mapper.Map<AccountMetaDataDto>(accountMetaData);

                return Ok(ApiResponse<AccountMetaDataDto>.Ok( "Record retrieved successfully.", dto));
            }
            catch (Exception)
            {
                return StatusCode( 500,ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AccountMetaDataDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AccountMetaDataDto>>> CreateAccountMetaData(CreateAccountMetaDataDto accountMetaDataDto)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var accountMetaData = _mapper.Map<AccountMetaData>(accountMetaDataDto);

                accountMetaData.UserId = userId;
                accountMetaData.CreatedAt = DateTime.UtcNow;

                _db.AccountMetaDatas.Add(accountMetaData);

                await _db.SaveChangesAsync();

                await _auditLogService.LogAsync( userId,"AccountMetaDataCreated",$"Account metadata created. Record ID: {accountMetaData.Id}");

                var dto = _mapper.Map<AccountMetaDataDto>(accountMetaData);

                var response = ApiResponse<AccountMetaDataDto>.CreatedAt("Created successfully.", dto);

                return CreatedAtAction( nameof(GetAccountMetaDataById),new { id = accountMetaData.Id },response);
            }
            catch (Exception ex)
            {
                return StatusCode( 500, ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AccountMetaDataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AccountMetaDataDto>>> UpdateAccountMetaData( int id,  UpdateAccountMetaDataDto accountMetaDataDto)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                if (id <= 0)
                {
                    return BadRequest( ApiResponse<object>.BadRequest( "Id must be greater than 0."));
                }

                if (accountMetaDataDto.Id != id)
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest(
                            "The ID in the URL must match the ID in the request body."));
                }

                var existingAccountMetaData =
               await _db.AccountMetaDatas.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

                if (existingAccountMetaData == null)
                {
                    return NotFound(ApiResponse<object>.NotFound( $"AccountMetaData with id {id} was not found."));
                }

                _mapper.Map( accountMetaDataDto, existingAccountMetaData);

               
                existingAccountMetaData.UserId = userId;
                existingAccountMetaData.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await _auditLogService.LogAsync(userId,"AccountMetaDataUpdated", $"Account metadata updated. Record ID: {id}");

                var dto = _mapper.Map<AccountMetaDataDto>(existingAccountMetaData);

                return Ok( ApiResponse<AccountMetaDataDto>.Ok("Updated successfully.", dto));
            }
            catch (Exception)
            {
                return StatusCode(500,ApiResponse<object>.Error( 500, "An unexpected error occurred."));
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccountMetaData(int id)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                if (id <= 0)
                    return BadRequest(ApiResponse<object>.BadRequest("Id must be greater than 0."));

                var accountMetaData =await _db.AccountMetaDatas.SingleOrDefaultAsync(e => e.Id == id && e.UserId == userId);

                if (accountMetaData == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"AccountMetaData with id {id} was not found."));
                }

                _db.AccountMetaDatas.Remove(accountMetaData);
                await _db.SaveChangesAsync();
                await _auditLogService.LogAsync( userId, "AccountMetaDataDeleted", $"Account metadata deleted. Record ID: {id}");

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500,ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }
    }
}
