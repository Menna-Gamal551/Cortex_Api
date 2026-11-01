namespace Cortex_Api.Data.Models.DTOs.DeviceDTOs
{

    public class DeviceDto
    {
        public int Id { get; set; }

        public required string DeviceUid { get; set; }

        public string? FirmwareVersion { get; set; }

        public DateTime? PairedAt { get; set; }

        public DateTime? LastConnectedAt { get; set; }

        public bool IsPaired { get; set; }
    }
}
