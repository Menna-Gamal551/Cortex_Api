using AutoMapper;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Cortex_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityEventController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public SecurityEventController(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SecurityEventDto>>>>GetAllSecurityEvents()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var events = await _db.SecurityEvents.AsNoTracking().Where(e => e.UserId == userId).OrderByDescending(e => e.OccurredAt).ToListAsync();

            var eventsDto =_mapper.Map<IEnumerable<SecurityEventDto>>(events);

            return Ok(ApiResponse<IEnumerable<SecurityEventDto>>.Ok("Security events retrieved successfully.",eventsDto));
        }
    }
}
