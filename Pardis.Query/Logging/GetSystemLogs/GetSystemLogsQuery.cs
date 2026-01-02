using MediatR;

namespace Pardis.Query.Logging.GetSystemLogs;

/// <summary>
/// Query for getting system logs
/// </summary>
public class GetSystemLogsQuery : IRequest<GetSystemLogsResult>
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Level { get; set; }
    public string? Source { get; set; }
    public string? EventId { get; set; }
    public string? UserId { get; set; }
    public string? RequestId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string Sort { get; set; } = "time_desc";
}

/// <summary>
/// Result for system logs query
/// </summary>
public class GetSystemLogsResult
{
    public List<SystemLogDto> Items { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// System log DTO
/// </summary>
public class SystemLogDto
{
    public Guid Id { get; set; }
    public DateTime Time { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? RequestId { get; set; }
    public string? EventId { get; set; }
    public string? Properties { get; set; }
}

/// <summary>
/// Pagination DTO
/// </summary>
public class PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}