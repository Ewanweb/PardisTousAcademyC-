using Microsoft.EntityFrameworkCore;
using Pardis.Application.Shopping.Contracts;
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

    public async Task<List<PaymentAttempt>> GetExpiredAttemptsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.PaymentAttempts
            .Where(pa => pa.ExpiresAt.HasValue && pa.ExpiresAt.Value < now && 
                        pa.Status != PaymentAttemptStatus.Paid && 
                        pa.Status != PaymentAttemptStatus.Expired)
            .ToListAsync(cancellationToken);
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