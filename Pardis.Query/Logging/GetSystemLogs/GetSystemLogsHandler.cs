using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Logging;

namespace Pardis.Query.Logging.GetSystemLogs;

/// <summary>
/// Handler for getting system logs
/// </summary>
public class GetSystemLogsHandler : IRequestHandler<GetSystemLogsQuery, GetSystemLogsResult>
{
    private readonly IRepository<SystemLog> _logRepository;

    public GetSystemLogsHandler(IRepository<SystemLog> logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<GetSystemLogsResult> Handle(GetSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _logRepository.Table
            .AsNoTracking()
            .AsQueryable();

        // Date range filters
        if (request.From.HasValue)
        {
            query = query.Where(l => l.Time >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(l => l.Time <= request.To.Value);
        }

        // Level filter
        if (!string.IsNullOrWhiteSpace(request.Level))
        {
            query = query.Where(l => l.Level.ToLower() == request.Level.ToLower());
        }

        // Source filter
        if (!string.IsNullOrWhiteSpace(request.Source))
        {
            query = query.Where(l => l.Source.ToLower().Contains(request.Source.ToLower()));
        }

        // EventId filter
        if (!string.IsNullOrWhiteSpace(request.EventId))
        {
            query = query.Where(l => l.EventId == request.EventId);
        }

        // UserId filter
        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            query = query.Where(l => l.UserId == request.UserId);
        }

        // RequestId filter
        if (!string.IsNullOrWhiteSpace(request.RequestId))
        {
            query = query.Where(l => l.RequestId == request.RequestId);
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(l => l.Message.ToLower().Contains(searchTerm) ||
                                   l.Source.ToLower().Contains(searchTerm));
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.Sort.ToLower() switch
        {
            "time" => query.OrderBy(l => l.Time),
            "time_desc" => query.OrderByDescending(l => l.Time),
            "level" => query.OrderBy(l => l.Level),
            "level_desc" => query.OrderByDescending(l => l.Level),
            "source" => query.OrderBy(l => l.Source),
            "source_desc" => query.OrderByDescending(l => l.Source),
            _ => query.OrderByDescending(l => l.Time)
        };

        // Apply pagination
        var logs = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs with masked properties
        var items = logs.Select(l => new SystemLogDto
        {
            Id = l.Id,
            Time = l.Time,
            Level = l.Level,
            Source = l.Source,
            Message = l.Message,
            UserId = l.UserId,
            RequestId = l.RequestId,
            EventId = l.EventId,
            Properties = l.GetMaskedProperties() // Use masked version for security
        }).ToList();

        return new GetSystemLogsResult
        {
            Items = items,
            Pagination = new PaginationDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = totalCount
            }
        };
    }
}