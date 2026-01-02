using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Contracts;

/// <summary>
/// رابط مخزن تلاش پرداخت
/// </summary>
public interface IPaymentAttemptRepository
{
    Task<PaymentAttempt?> GetByIdAsync(Guid paymentAttemptId, CancellationToken cancellationToken = default);
    Task<PaymentAttempt?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetPendingAdminApprovalAsync(CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetExpiredAttemptsAsync(CancellationToken cancellationToken = default);
    Task<PaymentAttempt> CreateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}