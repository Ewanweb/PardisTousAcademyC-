namespace Pardis.Domain.Audit;

public interface IPaymentAuditLogRepository : IRepository<PaymentAuditLog>
{
    Task<List<PaymentAuditLog>> GetByPaymentAttemptIdAsync(Guid paymentAttemptId, CancellationToken cancellationToken = default);
    Task<List<PaymentAuditLog>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<List<PaymentAuditLog>> GetByAdminUserIdAsync(string adminUserId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<List<PaymentAuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}