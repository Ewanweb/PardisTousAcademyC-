using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Courses;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت زمان‌بندی‌های دوره
/// </summary>
public class CourseScheduleRepository : Repository<CourseSchedule>, ICourseScheduleRepository
{
    public CourseScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<CourseSchedule>> GetSchedulesByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Course)
            .Where(s => s.CourseId == courseId && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserCourseSchedule>> GetStudentsByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserCourseSchedules
            .Include(ucs => ucs.User)
            .Include(ucs => ucs.CourseSchedule)
            .Where(ucs => ucs.CourseScheduleId == scheduleId)
            .OrderBy(ucs => ucs.User.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseSchedule?> GetScheduleWithStudentsAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Course)
            .Include(s => s.StudentEnrollments)
                .ThenInclude(se => se.User)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);
    }
}