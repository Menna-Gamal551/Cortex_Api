using Cortex_Api.Data;

namespace Cortex_Api.Services.IServices
{
    public interface ISecurityEventService
    {
        Task LogAsync(SecurityEventSeverity severity,string eventType,string? details = null,int? userId = null, int? deviceId = null);
    }
}
