using MediatR;
using Pardis.Domain.Attendance;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Attendance;
using Pardis.Domain.Users;

namespace Pardis.Query.Attendance;

public class GetStudentAttendanceReportHandler : IRequestHandler<GetStudentAttendanceReportQuery, StudentAttendanceReportDto>
{
    private readonly IStudentAttendanceRepository _attendanceRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public GetStudentAttendanceReportHandler(
        IStudentAttendanceRepository attendanceRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _attendanceRepository = attendanceRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<StudentAttendanceReportDto> Handle(GetStudentAttendanceReportQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            throw new ArgumentException("دوره یافت نشد", nameof(request.CourseId));

        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            throw new ArgumentException("دانشجو یافت نشد", nameof(request.StudentId));

        var attendances = await _attendanceRepository.GetAttendancesByStudentAndCourseAsync(
            request.StudentId, request.CourseId, cancellationToken);

        var presentCount = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var lateCount = attendances.Count(a => a.Status == AttendanceStatus.Late);
        var absentCount = attendances.Count(a => a.Status == AttendanceStatus.Absent);
        var totalSessions = attendances.Count;

        decimal attendancePercentage = 0;
        if (totalSessions > 0)
        {
            // درصد حضور: (حاضر + تأخیر) / کل جلسات
            attendancePercentage = (decimal)(presentCount + lateCount) / totalSessions * 100;
            attendancePercentage = Math.Round(attendancePercentage, 1);
        }

        return new StudentAttendanceReportDto
        {
            StudentId = student.Id,
            StudentName = student.FullName ?? student.UserName ?? "نامشخص",
            CourseId = course.Id,
            CourseName = course.Title,
            TotalSessions = totalSessions,
            PresentSessions = presentCount,
            AbsentSessions = absentCount,
            LateSessions = lateCount,
            AttendancePercentage = attendancePercentage,
            AttendanceDetails = attendances.Select(a => new StudentAttendanceDto
            {
                Id = a.Id,
                SessionId = a.SessionId,
                SessionTitle = a.Session.Title,
                SessionDate = a.Session.SessionDate,
                StudentId = a.StudentId,
                StudentName = student.FullName ?? student.UserName ?? "نامشخص",
                Status = a.Status,
                StatusDisplay = GetAttendanceStatusDisplay(a.Status),
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                AttendanceDuration = a.GetAttendanceDuration(),
                Note = a.Note,
                RecordedByUserName = a.RecordedByUser?.FullName ?? a.RecordedByUser?.UserName,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).OrderBy(a => a.SessionDate).ToList()
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
