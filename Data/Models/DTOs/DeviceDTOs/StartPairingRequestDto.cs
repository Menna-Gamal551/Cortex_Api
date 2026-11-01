
using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class StartPairingRequestDto
    {
        [Required]
        [MaxLength(100)]
        public required string DeviceUid { get; set; }

        [Required]
        public required string PublicKey { get; set; }

        [MaxLength(50)]
        public string? FirmwareVersion { get; set; }
    }
}
