namespace Pardis.Domain.Attendance;

/// <summary>
/// Repository برای مدیریت جلسات دوره
/// </summary>
public interface ICourseSessionRepository : IRepository<CourseSession>
{
    Task<List<CourseSession>> GetSessionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<CourseSession>> GetSessionsByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<int> GetSessionsCountByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
}