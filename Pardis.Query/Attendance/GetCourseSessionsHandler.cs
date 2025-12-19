using MediatR;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;

namespace Pardis.Query.Attendance;

/// <summary>
/// Handler برای دریافت جلسات یک دوره
/// </summary>
public class GetCourseSessionsHandler : IRequestHandler<GetCourseSessionsQuery, List<CourseSessionDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;

    public GetCourseSessionsHandler(
        ICourseRepository courseRepository,
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository,
        ICourseEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<CourseSessionDto>> Handle(GetCourseSessionsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            return [];

        var sessions = await _sessionRepository.GetSessionsByCourseIdAsync(request.CourseId, cancellationToken);
        
        // دریافت دانشجویان ثبت‌نام شده در دوره
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(request.CourseId, cancellationToken);

        var sessionDtos = new List<CourseSessionDto>();

        foreach (var session in sessions)
        {
            // دریافت آمار حضور و غیاب برای هر جلسه
            var attendances = await _attendanceRepository.GetAttendancesBySessionIdAsync(session.Id, cancellationToken);
            
            // ایجاد لیست دانشجویان با وضعیت حضور و غیاب فعلی
            var students = enrollments.Select(enrollment =>
            {
                var attendance = attendances.FirstOrDefault(a => a.StudentId == enrollment.StudentId);
                return new CourseStudentDto
                {
                    StudentId = enrollment.StudentId,
                    StudentName = enrollment.Student?.FullName ?? enrollment.Student?.UserName ?? "نامشخص",
                    StudentEmail = enrollment.Student?.Email,
                    StudentMobile = enrollment.Student?.Mobile,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    CurrentSessionStatus = attendance?.Status,
                    CurrentSessionStatusDisplay = attendance != null ? GetAttendanceStatusDisplay(attendance.Status) : null,
                    CheckInTime = attendance?.CheckInTime,
                    Note = attendance?.Note
                };
            }).ToList();
            
            var sessionDto = new CourseSessionDto
            {
                Id = session.Id,
                CourseId = session.CourseId,
                CourseName = course.Title,
                Title = session.Title,
                Description = session.Description,
                SessionDate = session.SessionDate,
                Duration = session.Duration,
                SessionNumber = session.SessionNumber,
                Status = session.Status,
                StatusDisplay = GetSessionStatusDisplay(session.Status),
                TotalStudents = enrollments.Count,
                PresentStudents = attendances.Count(a => a.Status == AttendanceStatus.Present),
                AbsentStudents = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                LateStudents = attendances.Count(a => a.Status == AttendanceStatus.Late),
                CreatedAt = session.CreatedAt,
                Students = students
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