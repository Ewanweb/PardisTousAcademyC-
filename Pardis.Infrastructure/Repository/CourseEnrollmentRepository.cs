using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Payments;
using Pardis.Application.Payments.Contracts;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت ثبت‌نام‌های دوره
/// </summary>
public class CourseEnrollmentRepository : Repository<CourseEnrollment>, ICourseEnrollmentRepository, IEnrollmentRepository
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

    // IEnrollmentRepository implementation
    public async Task<CourseEnrollment?> GetByUserAndCourseAsync(string userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await GetEnrollmentAsync(userId, courseId, cancellationToken);
    }

    public async Task<List<CourseEnrollment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await GetEnrollmentsByStudentIdAsync(userId, cancellationToken);
    }

    public async Task<List<CourseEnrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await GetEnrollmentsByCourseIdAsync(courseId, cancellationToken);
    }

    public async Task<CourseEnrollment> CreateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        await AddAsync(enrollment);
        return enrollment;
    }

    public async Task UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        Update(enrollment);
        await Task.CompletedTask;
    }

    public async Task<bool> IsUserEnrolledAsync(string userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId, cancellationToken);
    }

    // Additional methods required by IEnrollmentRepository
    async Task<CourseEnrollment?> Pardis.Application.Payments.Contracts.IEnrollmentRepository.GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(enrollmentId);
    }

    async Task Pardis.Application.Payments.Contracts.IEnrollmentRepository.SaveChangesAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
    }
}