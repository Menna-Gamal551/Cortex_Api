
using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class CompletePairingRequestDto
    {
        [Required]
        public required string PairingId { get; set; }

        [Required]
        public required string Signature { get; set; }
    }
}
