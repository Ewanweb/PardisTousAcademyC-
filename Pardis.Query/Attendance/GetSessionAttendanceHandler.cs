using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;

namespace Pardis.Query.Attendance;

/// <summary>
/// Handler برای دریافت حضور و غیاب یک جلسه
/// </summary>
public class GetSessionAttendanceHandler : IRequestHandler<GetSessionAttendanceQuery, SessionAttendanceDto?>
{
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public GetSessionAttendanceHandler(
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<SessionAttendanceDto?> Handle(GetSessionAttendanceQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null)
            return null;

        var attendances = await _attendanceRepository.GetAttendancesBySessionIdAsync(request.SessionId, cancellationToken);

        var sessionDto = new SessionAttendanceDto
        {
            Session = new CourseSessionDto
            {
                Id = session.Id,
                CourseId = session.CourseId,
                CourseName = session.Course?.Title ?? "نامشخص",
                Title = session.Title,
                SessionDate = session.SessionDate,
                Duration = session.Duration,
                SessionNumber = session.SessionNumber,
                Status = session.Status,
                StatusDisplay = GetSessionStatusDisplay(session.Status),
                TotalStudents = attendances.Count,
                PresentStudents = attendances.Count(a => a.Status == AttendanceStatus.Present),
                AbsentStudents = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                LateStudents = attendances.Count(a => a.Status == AttendanceStatus.Late)
            },
            Attendances = attendances.Select(attendance => new StudentAttendanceDto
            {
                Id = attendance.Id,
                SessionId = attendance.SessionId,
                SessionTitle = session.Title,
                SessionDate = session.SessionDate,
                StudentId = attendance.StudentId,
                StudentName = attendance.Student.FullName ?? attendance.Student.UserName ?? "نامشخص",
                Status = attendance.Status,
                StatusDisplay = GetAttendanceStatusDisplay(attendance.Status),
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                AttendanceDuration = attendance.GetAttendanceDuration(),
                Note = attendance.Note,
                RecordedByUserName = attendance.RecordedByUser?.FullName ?? attendance.RecordedByUser?.UserName
            }).ToList()
        };

        return sessionDto;
    }

    private static string GetSessionStatusDisplay(SessionStatus status)
    {
        return status switch
        {
            SessionStatus.Scheduled => "زمان‌بندی شده",
            SessionStatus.InProgress => "در حال برگزاری",
            SessionStatus.Completed => "تکمیل شده",
            SessionStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }

    private static string GetAttendanceStatusDisplay(AttendanceStatus status)
    {
        return status switch
        {
            AttendanceStatus.Present => "حاضر",
            AttendanceStatus.Absent => "غایب",
            AttendanceStatus.Late => "تأخیر",
            _ => status.ToString()
        };
    }
}