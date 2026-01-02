namespace Pardis.Domain.Logging;

/// <summary>
/// System log entry entity
/// </summary>
public class SystemLog : BaseEntity
{
    public DateTime Time { get; private set; }
    public string Level { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? UserId { get; private set; }
    public string? RequestId { get; private set; }
    public string? EventId { get; private set; }
    public string? Properties { get; private set; } // JSON string for additional properties

    // Private constructor for EF Core
    private SystemLog() { }

    public SystemLog(
        DateTime time,
        string level,
        string source,
        string message,
        string? userId = null,
        string? requestId = null,
        string? eventId = null,
        string? properties = null)
    {
        Time = time;
        Level = level;
        Source = source;
        Message = message;
        UserId = userId;
        RequestId = requestId;
        EventId = eventId;
        Properties = properties;
        CreatedAt = time;
        UpdatedAt = time;
    }

    /// <summary>
    /// Mask sensitive information in properties
    /// </summary>
    public string GetMaskedProperties()
    {
        if (string.IsNullOrEmpty(Properties))
            return Properties ?? string.Empty;

        // Simple masking - in production, you'd want more sophisticated masking
        var masked = Properties;
        
        // Mask common sensitive fields
        masked = System.Text.RegularExpressions.Regex.Replace(masked, 
            @"""(password|token|secret|key|authorization)""\s*:\s*""[^""]*""", 
            @"""$1"":""***MASKED***""", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return masked;
    }
}

/// <summary>
/// Log levels
/// </summary>
public static class LogLevel
{
    public const string Trace = "Trace";
    public const string Debug = "Debug";
    public const string Information = "Information";
    public const string Warning = "Warning";
    public const string Error = "Error";
    public const string Critical = "Critical";
}