using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Attendance;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت جلسات دوره
/// </summary>
public class CourseSessionRepository : Repository<CourseSession>, ICourseSessionRepository
{
    public CourseSessionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<CourseSession>> GetSessionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Course)
            .Include(s => s.Attendances)
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CourseSession>> GetSessionsByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Course)
            .Include(s => s.Schedule)
            .Include(s => s.Attendances)
            .Where(s => s.ScheduleId == scheduleId)
            .OrderBy(s => s.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetSessionsCountByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(s => s.CourseId == courseId)
            .CountAsync(cancellationToken);
    }
}