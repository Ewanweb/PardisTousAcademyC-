using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Attendance;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// پیاده‌سازی Repository برای مدیریت حضور و غیاب دانشجویان
/// </summary>
public class StudentAttendanceRepository : Repository<StudentAttendance>, IStudentAttendanceRepository
{
    public StudentAttendanceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<StudentAttendance>> GetAttendancesByStudentIdAsync(string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(a => a.Session)
                .ThenInclude(s => s.Course)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId)
            .OrderBy(a => a.Session.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StudentAttendance>> GetAttendancesBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(a => a.Session)
            .Include(a => a.Student)
            .Where(a => a.SessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StudentAttendance>> GetAttendancesByStudentAndCourseAsync(string studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(a => a.Session)
                .ThenInclude(s => s.Course)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId && a.Session.CourseId == courseId)
            .OrderBy(a => a.Session.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentAttendance?> GetAttendanceBySessionAndStudentAsync(Guid sessionId, string studentId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(a => a.Session)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.StudentId == studentId, cancellationToken);
    }
}