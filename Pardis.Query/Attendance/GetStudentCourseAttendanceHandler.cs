using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Users;
using Pardis.Domain.Courses;

namespace Pardis.Query.Attendance;

/// <summary>
/// Handler برای دریافت گزارش حضور دانشجو در یک دوره
/// </summary>
public class GetStudentCourseAttendanceHandler : IRequestHandler<GetStudentCourseAttendanceQuery, StudentCourseAttendanceReportDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public GetStudentCourseAttendanceHandler(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<StudentCourseAttendanceReportDto?> Handle(GetStudentCourseAttendanceQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            return null;

        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            return null;

        // دریافت تمام جلسات دوره
        var sessions = await _sessionRepository.GetSessionsByCourseIdAsync(request.CourseId, cancellationToken);
        
        // دریافت حضور و غیاب دانشجو در این دوره
        var attendances = await _attendanceRepository.GetAttendancesByStudentAndCourseAsync(request.StudentId, request.CourseId, cancellationToken);

        // ایجاد dictionary برای دسترسی سریع به حضور و غیاب
        var attendanceDict = attendances.ToDictionary(a => a.SessionId, a => a);

        // محاسبه آمار
        var totalSessions = sessions.Count;
        var presentSessions = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var lateSessions = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var absentSessions = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var attendanceRate = totalSessions > 0 ? (double)(presentSessions + lateSessions) / totalSessions * 100 : 0;

        // ایجاد گزارش جلسات
        var sessionDetails = sessions.Select(session =>
        {
            var attendance = attendanceDict.GetValueOrDefault(session.Id);
            return new SessionAttendanceDetailDto
            {
                SessionId = session.Id,
                SessionTitle = session.Title,
                SessionDate = session.SessionDate,
                Status = attendance?.Status.ToString() ?? "NotRecorded",
                CheckInTime = attendance?.CheckInTime,
                CheckOutTime = attendance?.CheckOutTime,
                Note = attendance?.Note
            };
        }).OrderBy(s => s.SessionDate).ToList();

        var report = new StudentCourseAttendanceReportDto
        {
            Student = new StudentBasicDto
            {
                Id = student.Id,
                FullName = student.FullName ?? student.UserName ?? "نامشخص",
                Email = student.Email ?? ""
            },
            Course = new CourseBasicDto
            {
                Id = course.Id,
                Title = course.Title
            },
            Summary = new AttendanceSummaryDto
            {
                TotalSessions = totalSessions,
                PresentSessions = presentSessions,
                LateSessions = lateSessions,
                AbsentSessions = absentSessions,
                AttendanceRate = attendanceRate
            },
            Sessions = sessionDetails
        };

        return report;
    }
}