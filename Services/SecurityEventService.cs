using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Services.IServices;

namespace Cortex_Api.Services
{
    public class SecurityEventService : ISecurityEventService
    {
        private readonly AppDbContext _db;

        public SecurityEventService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync( SecurityEventSeverity severity,string eventType,string? details = null,int? userId = null,int? deviceId = null)
        {
            var securityEvent = new SecurityEvent
            {
                Severity = severity,
                EventType = eventType,
                Details = details,
                UserId = userId,
                DeviceId = deviceId,
                OccurredAt = DateTime.UtcNow
            };

            _db.SecurityEvents.Add(securityEvent);
            await _db.SaveChangesAsync();
        }
    }
}

