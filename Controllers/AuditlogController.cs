using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cortex_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditlogController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public AuditlogController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditlogDto>>>> GetAllAuditlogs()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var logs = await _db.Auditlogs.Where(a => a.UserId == userId).OrderByDescending(a => a.OccuredAt).ToListAsync();

            var logsDto = _mapper.Map<IEnumerable<AuditlogDto>>(logs);

            return Ok(ApiResponse<IEnumerable<AuditlogDto>>.Ok( "Audit logs retrieved successfully.",logsDto));
        }


    }
}
