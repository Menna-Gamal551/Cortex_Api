
using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cortex_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // Extract current logged-in user's ID from JWT
        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdClaim, out userId);
        }


        // 1. Start Pairing
        [HttpPost]
        public async Task<IActionResult> StartPairing(
            [FromBody] StartPairingRequestDto request)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            try
            {
                var result = await _deviceService.StartPairingAsync(
                    userId,
                    request);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // 2. Complete Pairing
        [HttpPost]
        public async Task<IActionResult> CompletePairing(
            [FromBody] CompletePairingRequestDto request)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            try
            {
                bool isPaired = await _deviceService.CompletePairingAsync(userId, request);

                if (!isPaired)
                {
                    return BadRequest(new
                    {
                        message = "Device signature verification failed."
                    });
                }

                return Ok(new
                {
                    message = "Device paired successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // 3. Get Current User Device
        [HttpGet]
        public async Task<IActionResult> GetMyDevice()
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized();

            try
            {
                var device = await _deviceService.GetMyDeviceAsync(
                    userId);

                if (device == null)
                {
                    return NotFound(new
                    {
                        message = "No device is paired with this account."
                    });
                }

                return Ok(device);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
