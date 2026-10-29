using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs.DeviceDTOs
{
    public class PairDeviceRequestDto
    {
        [Required]
        [MaxLength(100)]
        public required string DeviceUid { get; set; }

        [MaxLength(50)]
        public string? FirmwareVersion { get; set; }
    }
}
