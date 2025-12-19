namespace Pardis.Domain.Attendance;

/// <summary>
/// Repository برای مدیریت حضور و غیاب دانشجویان
/// </summary>
public interface IStudentAttendanceRepository : IRepository<StudentAttendance>
{
    Task<List<StudentAttendance>> GetAttendancesByStudentIdAsync(string studentId, CancellationToken cancellationToken = default);
    Task<List<StudentAttendance>> GetAttendancesBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<StudentAttendance>> GetAttendancesByStudentAndCourseAsync(string studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<StudentAttendance?> GetAttendanceBySessionAndStudentAsync(Guid sessionId, string studentId, CancellationToken cancellationToken = default);
}