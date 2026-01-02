namespace Pardis.Domain.Payments;

/// <summary>
/// Repository برای مدیریت درخواست‌های پرداخت دستی
/// </summary>
public interface IManualPaymentRequestRepository : IRepository<ManualPaymentRequest>
{
    Task<ManualPaymentRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ManualPaymentRequest?> GetPendingRequestAsync(Guid courseId, string studentId, CancellationToken cancellationToken = default);
    Task<List<ManualPaymentRequest>> GetRequestsByStatusAsync(ManualPaymentStatus status, CancellationToken cancellationToken = default);
    Task<List<ManualPaymentRequest>> GetRequestsByStudentAsync(string studentId, CancellationToken cancellationToken = default);
    Task<List<ManualPaymentRequest>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}