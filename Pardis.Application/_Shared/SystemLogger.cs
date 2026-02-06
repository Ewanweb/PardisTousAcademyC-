using Pardis.Domain;
using Pardis.Domain.Logging;

namespace Pardis.Application._Shared;

public interface ISystemLogger
{
    Task LogInfoAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null);
    Task LogSuccessAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null);
    Task LogWarningAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null);
    Task LogErrorAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null);
}

public class SystemLogger : ISystemLogger
{
    private readonly IRepository<SystemLog> _logRepository;

    public SystemLogger(IRepository<SystemLog> logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogInfoAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null)
    {
        await LogAsync("info", message, source, userId, eventId, requestId, properties);
    }

    public async Task LogSuccessAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null)
    {
        await LogAsync("success", message, source, userId, eventId, requestId, properties);
    }

    public async Task LogWarningAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null)
    {
        await LogAsync("warning", message, source, userId, eventId, requestId, properties);
    }

    public async Task LogErrorAsync(string message, string source, string? userId = null, string? eventId = null, string? requestId = null, string? properties = null)
    {
        await LogAsync("error", message, source, userId, eventId, requestId, properties);
    }

    private async Task LogAsync(string level, string message, string source, string? userId, string? eventId, string? requestId, string? properties)
    {
        try
        {
            var log = new SystemLog(
                DateTime.UtcNow,
                level,
                source,
                message,
                userId,
                requestId,
                eventId,
                properties
            );

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync(CancellationToken.None);
        }
        catch
        {
            // Ignore logging errors
        }
    }
}
