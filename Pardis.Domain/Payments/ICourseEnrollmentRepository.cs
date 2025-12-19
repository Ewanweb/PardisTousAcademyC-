namespace Pardis.Domain.Payments;

/// <summary>
/// Repository برای مدیریت ثبت‌نام‌های دوره
/// </summary>
public interface ICourseEnrollmentRepository : IRepository<CourseEnrollment>
{
    Task<List<CourseEnrollment>> GetEnrollmentsByStudentIdAsync(string studentId, CancellationToken cancellationToken = default);
    Task<List<CourseEnrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseEnrollment?> GetEnrollmentAsync(string studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<List<CourseEnrollment>> GetEnrollmentsWithInstallmentsAsync(string studentId, CancellationToken cancellationToken = default);
}