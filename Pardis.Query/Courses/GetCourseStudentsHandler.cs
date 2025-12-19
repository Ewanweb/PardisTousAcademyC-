using MediatR;
using Pardis.Domain;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Payments;
using Pardis.Domain.Attendance;

namespace Pardis.Query.Courses;

/// <summary>
/// Handler برای دریافت دانشجویان یک دوره
/// </summary>
public class GetCourseStudentsHandler : IRequestHandler<GetCourseStudentsQuery, List<CourseStudentDto>>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly IStudentAttendanceRepository _attendanceRepository;

    public GetCourseStudentsHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        ICourseSessionRepository sessionRepository,
        IStudentAttendanceRepository attendanceRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _sessionRepository = sessionRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<List<CourseStudentDto>> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        // دریافت ثبت‌نام‌های دوره
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(request.CourseId, cancellationToken);

        // اعمال فیلتر جستجو
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            enrollments = enrollments.Where(e => 
                (e.Student.FullName?.ToLower().Contains(searchTerm) ?? false) ||
                (e.Student.Email?.ToLower().Contains(searchTerm) ?? false) ||
                (e.Student.Mobile?.Contains(searchTerm) ?? false)).ToList();
        }

        // صفحه‌بندی
        var pagedEnrollments = enrollments
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new List<CourseStudentDto>();

        foreach (var enrollment in pagedEnrollments)
        {
            // محاسبه آمار حضور و غیاب
            var attendanceStats = await GetStudentAttendanceStats(enrollment.StudentId, enrollment.CourseId, cancellationToken);

            var studentDto = new CourseStudentDto
            {
                Id = enrollment.StudentId,
                FullName = enrollment.Student.FullName ?? enrollment.Student.UserName ?? "نامشخص",
                Email = enrollment.Student.Email ?? "",
                PhoneNumber = enrollment.Student.Mobile ?? "",
                ProfileImage = enrollment.Student.Avatar, // اضافه کردن تصویر پروفایل
                EnrollmentDate = enrollment.EnrollmentDate,
                EnrollmentStatus = enrollment.EnrollmentStatus.ToString(),
                EnrollmentStatusDisplay = GetEnrollmentStatusDisplay(enrollment.EnrollmentStatus),
                PaymentStatus = enrollment.PaymentStatus.ToString(),
                PaymentStatusDisplay = GetPaymentStatusDisplay(enrollment.PaymentStatus),
                TotalAmount = enrollment.TotalAmount,
                PaidAmount = enrollment.PaidAmount,
                RemainingAmount = enrollment.GetRemainingAmount(),
                PaymentPercentage = enrollment.GetPaymentPercentage(),
                TotalSessions = attendanceStats.TotalSessions,
                AttendedSessions = attendanceStats.AttendedSessions,
                AbsentSessions = attendanceStats.AbsentSessions,
                AttendancePercentage = attendanceStats.AttendancePercentage
            };

            result.Add(studentDto);
        }

        return result;
    }

    private async Task<(int TotalSessions, int AttendedSessions, int AbsentSessions, double AttendancePercentage)> 
        GetStudentAttendanceStats(string studentId, Guid courseId, CancellationToken cancellationToken)
    {
        // دریافت تعداد کل جلسات دوره
        var totalSessions = await _sessionRepository.GetSessionsCountByCourseIdAsync(courseId, cancellationToken);

        if (totalSessions == 0)
            return (0, 0, 0, 0);

        // دریافت آمار حضور دانشجو
        var attendances = await _attendanceRepository.GetAttendancesByStudentAndCourseAsync(studentId, courseId, cancellationToken);

        var attendanceStats = attendances
            .GroupBy(a => a.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        var presentSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Present, 0);
        var lateSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Late, 0);
        var absentSessions = attendanceStats.GetValueOrDefault(AttendanceStatus.Absent, 0);

        var attendedSessions = presentSessions + lateSessions;
        var attendancePercentage = totalSessions > 0 ? (double)attendedSessions / totalSessions * 100 : 0;

        return (totalSessions, attendedSessions, absentSessions, attendancePercentage);
    }

    private static string GetEnrollmentStatusDisplay(EnrollmentStatus status)
    {
        return status switch
        {
            EnrollmentStatus.Active => "فعال",
            EnrollmentStatus.Completed => "تکمیل شده",
            EnrollmentStatus.Cancelled => "لغو شده",
            EnrollmentStatus.Suspended => "تعلیق شده",
            _ => status.ToString()
        };
    }

    private static string GetPaymentStatusDisplay(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Unpaid => "پرداخت نشده",
            PaymentStatus.Partial => "پرداخت جزئی",
            PaymentStatus.Paid => "پرداخت کامل",
            _ => status.ToString()
        };
    }
}