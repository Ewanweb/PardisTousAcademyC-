using Microsoft.EntityFrameworkCore;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Shopping;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی مخزن تلاش پرداخت
/// </summary>
public class PaymentAttemptRepository : IPaymentAttemptRepository
{
    private readonly AppDbContext _context;

    public PaymentAttemptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentAttempt?> GetByIdAsync(Guid paymentAttemptId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Include(pa => pa.User)
            .FirstOrDefaultAsync(pa => pa.Id == paymentAttemptId, cancellationToken);
    }

    public async Task<PaymentAttempt?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Include(pa => pa.User)
            .FirstOrDefaultAsync(pa => pa.TrackingCode == trackingCode, cancellationToken);
    }

    public async Task<List<PaymentAttempt>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Where(pa => pa.UserId == userId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAttempt>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Where(pa => pa.OrderId == orderId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAttempt>> GetPendingAdminApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Include(pa => pa.User)
            .Where(pa => pa.Status == PaymentAttemptStatus.AwaitingAdminApproval)
            .OrderBy(pa => pa.ReceiptUploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAttempt>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Include(pa => pa.User)
            .OrderBy(pa => pa.ReceiptUploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<PaymentAttempt>> GetAllPagedAsync(PaginationRequest pagination, string? searchTerm, int? status, CancellationToken cancellationToken = default)
    {
        var normalized = PaginationHelper.Normalize(pagination);

        var query = _context.PaymentAttempts
            .Include(pa => pa.Order)
            .Include(pa => pa.User)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToLower();
            query = query.Where(pa =>
                (pa.User.FullName != null && pa.User.FullName.ToLower().Contains(normalized)) ||
                (pa.Order.OrderNumber != null && pa.Order.OrderNumber.ToLower().Contains(normalized)) ||
                (pa.TrackingCode != null && pa.TrackingCode.ToLower().Contains(normalized)));
        }

        if (status.HasValue)
        {
            query = query.Where(pa => (int)pa.Status == status.Value);
        }

        var statusCounts = await query
            .GroupBy(pa => pa.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var total = statusCounts.Sum(item => item.Count);
        normalized = PaginationHelper.ClampPage(normalized, total);

        var items = await query
            .OrderByDescending(pa => pa.ReceiptUploadedAt ?? pa.CreatedAt)
            .ThenByDescending(pa => pa.Id)
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        var stats = new Dictionary<string, int>
        {
            ["total"] = total,
            ["pending"] = statusCounts.FirstOrDefault(s => s.Status == PaymentAttemptStatus.AwaitingAdminApproval)?.Count ?? 0,
            ["approved"] = statusCounts.FirstOrDefault(s => s.Status == PaymentAttemptStatus.Paid)?.Count ?? 0,
            ["rejected"] = statusCounts.FirstOrDefault(s => s.Status == PaymentAttemptStatus.Failed)?.Count ?? 0
        };

        return PaginationHelper.Create(items, normalized, total, stats);
    }

    public async Task<List<PaymentAttempt>> GetExpiredAttemptsAsync(CancellationToken cancellationToken = default)
    {
        // For manual payments, we don't have expiry logic
        // Return empty list since manual payments don't expire
        return new List<PaymentAttempt>();
    }

    public async Task<PaymentAttempt> CreateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default)
    {
        var entry = await _context.PaymentAttempts.AddAsync(paymentAttempt, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default)
    {
        _context.PaymentAttempts.Update(paymentAttempt);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
