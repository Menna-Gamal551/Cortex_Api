
namespace Cortex_Api.Data.Models.DTOs
{
    public class StartPairingResponseDto
    {
        public required string PairingId { get; set; }

        public required string Challenge { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
