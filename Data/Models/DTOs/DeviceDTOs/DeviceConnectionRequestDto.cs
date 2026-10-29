
using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Data.Models.DTOs.DeviceDTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cortex_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(AppDbContext db,IMapper mapper,IAuditLogService auditLogService, ILogger<DeviceController> logger)
        {
            _db = db;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }

        // GET: api/Device
        [HttpGet]
        [ProducesResponseType(
            typeof(ApiResponse<DeviceResponseDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DeviceResponseDto>>>
            GetMyDevice()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var user = await _db.Users
                    .AsNoTracking()
                    .Include(u => u.device)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return Unauthorized();

                if (user.device == null)
                {
                    return NotFound(
                        ApiResponse<object>.NotFound(
                            "No device is paired with this account."));
                }

                var dto = _mapper.Map<DeviceResponseDto>(user.device);

                return Ok(
                    ApiResponse<DeviceResponseDto>.Ok(
                        "Device retrieved successfully.",
                        dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve device for user {UserId}.",
                    userId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Error(
                        500,
                        "An unexpected error occurred."));
            }
        }

        // POST: api/Device/pair
        [HttpPost("pair")]
        [ProducesResponseType(
            typeof(ApiResponse<DeviceResponseDto>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status409Conflict)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DeviceResponseDto>>>
            PairDevice([FromBody] PairDeviceRequestDto pairDto)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            // [ApiController] handles invalid model validation automatically.
            var deviceUid = pairDto.DeviceUid.Trim();

            try
            {
                var user = await _db.Users
                    .Include(u => u.device)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return Unauthorized();

                if (user.DeviceId != null || user.device != null)
                {
                    return Conflict(
                        ApiResponse<object>.Error(
                            409,
                            "This account already has a paired device."));
                }

                var device = await _db.Devices
                    .SingleOrDefaultAsync(
                        d => d.DeviceUid == deviceUid);

                if (device != null)
                {
                    var alreadyAssigned = await _db.Users
                        .AnyAsync(u => u.DeviceId == device.Id);

                    if (alreadyAssigned || device.IsPaired)
                    {
                        return Conflict(
                            ApiResponse<object>.Error(
                                409,
                                "This device is already paired."));
                    }
                }
                else
                {
                    device = new Device
                    {
                        DeviceUid = deviceUid
                    };

                    _db.Devices.Add(device);
                }

                var now = DateTime.UtcNow;

                device.FirmwareVersion = pairDto.FirmwareVersion?.Trim();
                device.IsPaired = true;
                device.PairedAt = now;
                device.LastConnectedAt = null;

                // Associate this device with the authenticated user.
                user.device = device;

                await _db.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    userId,
                    "DevicePaired",
                    $"Device paired. Device ID: {device.Id}");

                var dto = _mapper.Map<DeviceResponseDto>(device);

                var response = ApiResponse<DeviceResponseDto>.CreatedAt(
                    "Device paired successfully.",
                    dto);

                return CreatedAtAction(
                    nameof(GetMyDevice),
                    routeValues: null,
                    value: response);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Database update failed while pairing a device for user {UserId}.",
                    userId);

                return Conflict(
                    ApiResponse<object>.Error(
                        409,
                        "The device could not be paired. Check whether it is already assigned and try again."));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while pairing a device for user {UserId}.",
                    userId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Error(
                        500,
                        "An unexpected error occurred."));
            }
        }

        // POST: api/Device/connection
        [HttpPost("connection")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ReportConnection(
            [FromBody] DeviceConnectionRequestDto connectionDto)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var user = await _db.Users
                    .Include(u => u.device)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return Unauthorized();

                if (user.device == null || !user.device.IsPaired)
                {
                    return NotFound(
                        ApiResponse<object>.NotFound(
                            "No paired device was found for this account."));
                }

                user.device.LastConnectedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(
                    connectionDto.FirmwareVersion))
                {
                    user.device.FirmwareVersion =
                        connectionDto.FirmwareVersion.Trim();
                }

                await _db.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    userId,
                    "DeviceConnectionReported",
                    $"Device connection reported by client. Device ID: {user.device.Id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to record a device connection report for user {UserId}.",
                    userId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Error(
                        500,
                        "An unexpected error occurred."));
            }
        }

        // DELETE: api/Device
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ApiResponse<object>),
            StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UnpairDevice()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var user = await _db.Users
                    .Include(u => u.device)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return Unauthorized();

                if (user.device == null)
                {
                    return NotFound(
                        ApiResponse<object>.NotFound(
                            "No device is paired with this account."));
                }

                var device = user.device;
                var deviceId = device.Id;

                // Detach the device from this account.
                user.device = null;
                user.DeviceId = null;

                // Keep the device record; do not delete it.
                device.IsPaired = false;

                await _db.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    userId,
                    "DeviceUnpaired",
                    $"Device unpaired. Device ID: {deviceId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to unpair the device for user {UserId}.",
                    userId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Error(
                        500,
                        "An unexpected error occurred."));
            }
        }
    }
}
