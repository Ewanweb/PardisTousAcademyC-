using Pardis.Domain.Payments;

namespace Pardis.Application.Payments.Contracts;

/// <summary>
/// رابط مخزن ثبت‌نام دوره
/// </summary>
public interface IEnrollmentRepository
{
    Task<CourseEnrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<CourseEnrollment?> GetByUserAndCourseAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<List<CourseEnrollment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<CourseEnrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseEnrollment> CreateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);
    Task<bool> IsUserEnrolledAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}