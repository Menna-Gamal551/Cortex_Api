namespace Cortex_Api.Services.IServices
{
    public  interface  IAuditLogService
    {
        Task LogAsync(int userId, string action, string? details = null);
    }
}
