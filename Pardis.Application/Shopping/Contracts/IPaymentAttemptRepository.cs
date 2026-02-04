using Pardis.Domain.Shopping;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pardis.Application.Shopping.Contracts;

/// <summary>
/// رابط مخزن تلاش پرداخت
/// </summary>
public interface IPaymentAttemptRepository
{
    IQueryable<PaymentAttempt> Table { get; }
    Task<PaymentAttempt?> GetByIdAsync(Guid paymentAttemptId, CancellationToken cancellationToken = default);
    Task<PaymentAttempt?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetPendingAdminApprovalAsync(CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetExpiredAttemptsAsync(CancellationToken cancellationToken = default);
    Task<PaymentAttempt> CreateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetAll(CancellationToken cancellationToken = default);
    Task<Pardis.Application._Shared.Pagination.PagedResult<PaymentAttempt>> GetAllPagedAsync(
        Pardis.Application._Shared.Pagination.PaginationRequest pagination,
        string? searchTerm,
        int? status,
        CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
