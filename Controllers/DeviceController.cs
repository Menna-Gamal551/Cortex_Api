using AutoMapper;
using Cortex_Api.Data;
using Cortex_Api.Data.Enums;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Data.Models.DTOs.DeviceDTOs;
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
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ISecurityEventService _securityEventService;

        public DeviceController(
            AppDbContext db,
            IMapper mapper,
            IAuditLogService auditLogService,
            ISecurityEventService securityEventService)
        {
            _db = db;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _securityEventService = securityEventService;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }

        // GET api/Device
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<DeviceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DeviceResponseDto>>> GetMyDevice()
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var device = await _db.Devices
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.UserId == userId);

                if (device == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("No device is paired with this account."));
                }

                var dto = _mapper.Map<DeviceResponseDto>(device);

                return Ok(ApiResponse<DeviceResponseDto>.Ok("Device retrieved successfully.", dto));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }

        // POST api/Device/pair
        [HttpPost("pair")]
        [ProducesResponseType(typeof(ApiResponse<DeviceResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DeviceResponseDto>>> PairDevice(PairDeviceRequestDto pairDto)
        {
            try
            {
                // الخطوة 1: نعرف مين اليوزر
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                if (!await _db.Users.AnyAsync(u => u.Id == userId))
                    return Unauthorized();

                // الخطوة 2: الحساب لازم يكون فاضي من أي جهاز
                if (await _db.Devices.AnyAsync(d => d.UserId == userId))
                {
                    return Conflict(ApiResponse<object>.Error(409, "This account already has a paired device."));
                }

                // الخطوة 3: الجهاز ده متسجل عند حد؟ (لو موجود يبقى له صاحب دايمًا)
                var existing = await _db.Devices
                    .AsNoTracking()
                    .Where(d => d.DeviceUid == pairDto.DeviceUid)
                    .Select(d => new { d.Id, d.UserId })
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    // حدث أمني لصاحب الجهاز (من غير ما نكشف هوية اللي حاول)
                    await _securityEventService.LogAsync(
                        SecurityEventSeverity.Medium,
                        "DevicePairingAttemptBlocked",
                        "Someone tried to pair your device with another account. If this wasn't you, review your device security.",
                        existing.UserId,
                        existing.Id);

                    return Conflict(ApiResponse<object>.Error(409, "This device can't be paired with this account."));
                }

                // الخطوة 4: ننشئ الجهاز، والسيرفر هو اللي يملّا الحقول ويربطه باليوزر
                var now = DateTime.UtcNow;

                var device = new Device
                {
                    DeviceUid = pairDto.DeviceUid,
                    FirmwareVersion = pairDto.FirmwareVersion,
                    UserId = userId,
                    IsPaired = true,
                    PairedAt = now,
                    LastConnectedAt = now
                };

                _db.Devices.Add(device);

                // الخطوة 5: نحفظ (الـ Unique Indexes هم خط الدفاع الأخير لو طلبين جم مع بعض)
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return Conflict(ApiResponse<object>.Error(409, "Device pairing conflict. Please try again."));
                }

                // الخطوة 6: نسجّل الحدث بعد ما اتأكدنا إنه اتحفظ
                await _auditLogService.LogAsync(
                    userId,
                    "DevicePaired",
                    "Device paired successfully.",
                    device.Id);

                // الخطوة 7: نحوّل لـ DTO ونرجّع 201
                var dto = _mapper.Map<DeviceResponseDto>(device);

                var response = ApiResponse<DeviceResponseDto>.CreatedAt("Device paired successfully.", dto);

                return CreatedAtAction(nameof(GetMyDevice), response);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }

        // POST api/Device/connection
        [HttpPost("connection")]
        [ProducesResponseType(typeof(ApiResponse<DeviceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DeviceResponseDto>>> ReportConnection(DeviceConnectionRequestDto connectionDto)
        {
            try
            {
                // الخطوة 1: نعرف مين اليوزر
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                // الخطوة 2: نجيب جهاز اليوزر ده (من غير AsNoTracking لأننا هنعدّل عليه)
                var device = await _db.Devices.SingleOrDefaultAsync(d => d.UserId == userId);

                if (device == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("No device is paired with this account."));
                }

                // الخطوة 3: الـ uid المبعوت لازم يطابق جهاز اليوزر
                if (!string.Equals(device.DeviceUid, connectionDto.DeviceUid, StringComparison.Ordinal))
                {
                    await _securityEventService.LogAsync(
                        SecurityEventSeverity.Medium,
                        "DeviceConnectionMismatch",
                        "A connection report was received for a device that doesn't match your paired device. If this wasn't you, review your account security.",
                        userId,
                        device.Id);

                    return Conflict(ApiResponse<object>.Error(409, "The reported device doesn't match your paired device."));
                }

                // الخطوة 4: نحدّث الحقول (في الـ memory لسه)
                device.LastConnectedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(connectionDto.FirmwareVersion))
                    device.FirmwareVersion = connectionDto.FirmwareVersion;

                // الخطوة 5: نحفظ في الـ database
                // لو فشل، الـ exception هيروح للـ catch الكبير (500) ومش هنوصل لتسجيل الحدث
                await _db.SaveChangesAsync();

                // الخطوة 6: نسجّل الحدث بعد ما اتأكدنا إنه اتحفظ، ونرجّع الجهاز بعد التحديث
                await _auditLogService.LogAsync(
                    userId,
                    "DeviceConnected",
                    $"Device connected. Firmware version: {device.FirmwareVersion ?? "unknown"}",
                    device.Id);

                var configuration = _mapper.ConfigurationProvider;

                var dto = _mapper.Map<DeviceResponseDto>(device);

                return Ok(ApiResponse<DeviceResponseDto>.Ok("Device connection recorded successfully.", dto));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(
                    500,
                    ApiResponse<object>.Error(500, "An unexpected error occurred.")
                );
            }
        }

        // DELETE api/Device
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> UnpairDevice()
        {
            try
            {
                // الخطوة 1: نعرف مين اليوزر
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                // الخطوة 2: نجيب جهاز اليوزر ده (من غير AsNoTracking لأننا هنمسحه)
                var device = await _db.Devices.SingleOrDefaultAsync(d => d.UserId == userId);

                if (device == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("No device is paired with this account."));
                }

                // الخطوة 3: نحتفظ برقم الجهاز قبل المسح، وبعدين نمسح
                var deletedDeviceId = device.Id;

                _db.Devices.Remove(device);

                // لو فشل، الـ exception هيروح للـ catch الكبير (500) ومش هنوصل لتسجيل الحدث
                await _db.SaveChangesAsync();

                // الخطوة 4: نسجّل الحدث بعد المسح. DeviceId = null لأن الصف اتمسح،
                // ورقم الجهاز في الـ Details
                await _auditLogService.LogAsync(
                    userId,
                    "DeviceUnpaired",
                    $"Device unpaired. Device ID: {deletedDeviceId}",
                    deviceId: null);

                // الخطوة 5: نجاح من غير محتوى
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "An unexpected error occurred."));
            }
        }
    }
}