namespace Cortex_Api.Data.Models.DTOs
{
    public class AuditlogDto
    {
        public int Id { get; set; }

        public required string Action { get; set; }

        public DateTime OccurredAt { get; set; }

        public string? Details { get; set; }
    }
}
