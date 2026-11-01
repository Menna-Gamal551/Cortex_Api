using Cortex_Api.Data.Models.DTOs;
using Cortex_Api.Data.Models.DTOs.DeviceDTOs;

namespace Cortex_Api.Services.IServices
{
    public interface IDeviceService
    {
        Task<StartPairingResponseDto> StartPairingAsync(
            int userId,
            StartPairingRequestDto request);

        Task<bool> CompletePairingAsync(
            int userId,
            CompletePairingRequestDto request);

        Task<DeviceDto?> GetMyDeviceAsync(int userId);
    }
}
