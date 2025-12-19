using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Payments;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت ثبت‌نام‌های دوره
/// </summary>
public class CourseEnrollmentRepository : Repository<CourseEnrollment>, ICourseEnrollmentRepository
{
    public CourseEnrollmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<CourseEnrollment>> GetEnrollmentsByStudentIdAsync(string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Include(e => e.InstallmentPayments)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CourseEnrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Include(e => e.InstallmentPayments)
            .Where(e => e.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseEnrollment?> GetEnrollmentAsync(string studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Include(e => e.InstallmentPayments)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<List<CourseEnrollment>> GetEnrollmentsWithInstallmentsAsync(string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Include(e => e.InstallmentPayments)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }
}