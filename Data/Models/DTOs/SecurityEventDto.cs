namespace Cortex_Api.Data.Models.DTOs
{
    public class SecurityEventDto
    {
        public int Id { get; set; }

        public required SecurityEventSeverity Severity { get; set; }

        public required string EventType { get; set; }

        public DateTime OccuredAt { get; set; }

        public string? Details { get; set; }
    }
}
