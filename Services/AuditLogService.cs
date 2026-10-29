using Cortex_Api.Data;
using Cortex_Api.Data.Models;
using Cortex_Api.Services.IServices;

namespace Cortex_Api.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _db;

        public AuditLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(int userId, string action, string? details = null)
        {
            var auditLog = new Auditlog
            {
                UserId = userId,
                Action = action,
                OccuredAt = DateTime.UtcNow,
                Details = details
            };

            _db.Auditlogs.Add(auditLog);

            await _db.SaveChangesAsync();
        }
    }
}
