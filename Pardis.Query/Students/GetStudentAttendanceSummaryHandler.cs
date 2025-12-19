using MediatR;
using Pardis.Domain.Dto.Students;
using Pardis.Domain.Payments;
using Pardis.Domain.Attendance;

namespace Pardis.Query.Students;

/// <summary>
/// Handler برای دریافت خلاصه حضور و غیاب دانشجو
/// </summary>
public class GetStudentAttendanceSummaryHandler : IRequestHandler<GetStudentAttendanceSummaryQuery, List<StudentCourseAttendanceSummaryDto>>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public GetStudentAttendanceSummaryHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<List<StudentCourseAttendanceSummaryDto>> Handle(GetStudentAttendanceSummaryQuery request, CancellationToken cancellationToken)
    {
        // دریافت دوره‌هایی که دانشجو در آنها ثبت‌نام کرده
        var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(request.StudentId, cancellationToken);

        var result = new List<StudentCourseAttendanceSummaryDto>();

        foreach (var enrollment in enrollments)
        {
            var course = enrollment.Course;
            
            // دریافت تعداد کل جلسات دوره
            var totalSessions = await _sessionRepository.GetSessionsCountByCourseIdAsync(course.Id, cancellationToken);

            if (totalSessions == 0)
            {
                result.Add(new StudentCourseAttendanceSummaryDto
                {
                    CourseId = course.Id,
                    CourseName = course.Title,
                    TotalSessions = 0,
                    AttendedSessions = 0,
                    PresentSessions = 0,
                    LateSessions = 0,
                    AbsentSessions = 0,
                    AttendancePercentage = 0
                });
                continue;
            }

            // دریافت آمار حضور دانشجو در این دوره
            var attendances = await _attendanceRepository.GetAttendancesByStudentAndCourseAsync(request.StudentId, course.Id, cancellationToken);

            var attendanceStats = attendances
                .GroupBy(a => a.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var presentSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Present, 0);
            var lateSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Late, 0);
            var absentSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Absent, 0);

            var attendedSessions = presentSessions + lateSessions;
            var attendancePercentage = totalSessions > 0 ? (double)attendedSessions / totalSessions * 100 : 0;

            result.Add(new StudentCourseAttendanceSummaryDto
            {
                CourseId = course.Id,
                CourseName = course.Title,
                TotalSessions = totalSessions,
                AttendedSessions = attendedSessions,
                PresentSessions = presentSessions,
                LateSessions = lateSessions,
                AbsentSessions = absentSessions,
                AttendancePercentage = attendancePercentage
            });
        }

        return result.OrderBy(r => r.CourseName).ToList();
    }
}