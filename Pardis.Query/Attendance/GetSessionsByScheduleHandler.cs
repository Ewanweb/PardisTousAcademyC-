using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Courses;

namespace Pardis.Query.Attendance;

/// <summary>
/// Handler برای دریافت جلسات یک زمان‌بندی خاص
/// </summary>
public class GetSessionsByScheduleHandler : IRequestHandler<GetSessionsByScheduleQuery, List<CourseSessionDto>>
{
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly ICourseScheduleRepository _scheduleRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public GetSessionsByScheduleHandler(
        ICourseSessionRepository sessionRepository,
        ICourseScheduleRepository scheduleRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _sessionRepository = sessionRepository;
        _scheduleRepository = scheduleRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<List<CourseSessionDto>> Handle(GetSessionsByScheduleQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetSessionsByScheduleIdAsync(request.ScheduleId, cancellationToken);
        
        // دریافت دانشجویان زمان‌بندی برای محاسبه آمار
        var scheduleStudents = await _scheduleRepository.GetStudentsByScheduleIdAsync(request.ScheduleId, cancellationToken);
        var totalStudentsInSchedule = scheduleStudents.Count;

        var sessionDtos = new List<CourseSessionDto>();

        foreach (var session in sessions)
        {
            // دریافت آمار حضور و غیاب برای هر جلسه
            var attendances = await _attendanceRepository.GetAttendancesBySessionIdAsync(session.Id, cancellationToken);
            
            var sessionDto = new CourseSessionDto
            {
                Id = session.Id,
                CourseId = session.CourseId,
                ScheduleId = session.ScheduleId,
                CourseName = session.Course?.Title ?? "نامشخص",
                ScheduleTitle = session.Schedule?.Title,
                Title = session.Title,
                Description = session.Description,
                SessionDate = session.SessionDate,
                Duration = session.Duration,
                SessionNumber = session.SessionNumber,
                Status = session.Status,
                StatusDisplay = GetSessionStatusDisplay(session.Status),
                TotalStudents = totalStudentsInSchedule,
                PresentStudents = attendances.Count(a => a.Status == AttendanceStatus.Present),
                AbsentStudents = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                LateStudents = attendances.Count(a => a.Status == AttendanceStatus.Late),
                CreatedAt = session.CreatedAt,
                Students = [] // خالی برای این endpoint
            };

            sessionDtos.Add(sessionDto);
        }

        return sessionDtos.OrderBy(s => s.SessionNumber).ToList();
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
}