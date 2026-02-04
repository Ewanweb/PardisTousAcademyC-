using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Audit;

namespace Pardis.Infrastructure.Repository;

public class PaymentAuditLogRepository : Repository<PaymentAuditLog>, IPaymentAuditLogRepository
{
    private readonly AppDbContext _context;

    public PaymentAuditLogRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<PaymentAuditLog>> GetByPaymentAttemptIdAsync(
        Guid paymentAttemptId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAuditLogs
            .Where(l => l.PaymentAttemptId == paymentAttemptId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAuditLog>> GetByUserIdAsync(
        string userId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 50 : pageSize;

        return await _context.PaymentAuditLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAuditLog>> GetByAdminUserIdAsync(
        string adminUserId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 50 : pageSize;

        return await _context.PaymentAuditLogs
            .Where(l => l.AdminUserId == adminUserId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAuditLog>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var fromUtc = from.Kind == DateTimeKind.Utc ? from : from.ToUniversalTime();
        var toUtc = to.Kind == DateTimeKind.Utc ? to : to.ToUniversalTime();

        return await _context.PaymentAuditLogs
            .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt <= toUtc)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
