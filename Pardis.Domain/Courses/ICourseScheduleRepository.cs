namespace Pardis.Domain.Courses;

/// <summary>
/// Repository برای مدیریت زمان‌بندی‌های دوره
/// </summary>
public interface ICourseScheduleRepository : IRepository<CourseSchedule>
{
    Task<List<CourseSchedule>> GetSchedulesByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<UserCourseSchedule>> GetStudentsByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<CourseSchedule?> GetScheduleWithStudentsAsync(Guid scheduleId, CancellationToken cancellationToken = default);
}