using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Payments;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت درخواست‌های پرداخت دستی
/// </summary>
public class ManualPaymentRequestRepository : Repository<ManualPaymentRequest>, IManualPaymentRequestRepository
{
    public ManualPaymentRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ManualPaymentRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(m => m.Course)
            .Include(m => m.Student)
            .Include(m => m.AdminReviewer)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<ManualPaymentRequest?> GetPendingRequestAsync(Guid courseId, string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .FirstOrDefaultAsync(m => m.CourseId == courseId && 
                                     m.StudentId == studentId &&
                                     (m.Status == ManualPaymentStatus.PendingReceipt || 
                                      m.Status == ManualPaymentStatus.PendingApproval), 
                                cancellationToken);
    }

    public async Task<List<ManualPaymentRequest>> GetRequestsByStatusAsync(ManualPaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(m => m.Course)
            .Include(m => m.Student)
            .Include(m => m.AdminReviewer)
            .Where(m => m.Status == status)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ManualPaymentRequest>> GetRequestsByStudentAsync(string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(m => m.Course)
            .Include(m => m.Student)
            .Include(m => m.AdminReviewer)
            .Where(m => m.StudentId == studentId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ManualPaymentRequest>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(m => m.Course)
            .Include(m => m.Student)
            .Include(m => m.AdminReviewer)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}