using MediatR;
using Pardis.Domain;
using Pardis.Domain.Dto.Students;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Students;

/// <summary>
/// Handler برای دریافت پروفایل کامل دانشجو
/// </summary>
public class GetStudentProfileHandler : IRequestHandler<GetStudentProfileQuery, StudentProfileDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;

    public GetStudentProfileHandler(IUserRepository userRepository, ICourseEnrollmentRepository enrollmentRepository)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<StudentProfileDto?> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);

        if (student == null)
            return null;

        // دریافت اطلاعات ثبت‌نام‌ها
        var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(request.StudentId, cancellationToken);

        // محاسبه آمارهای کلی
        var totalAmountOwed = enrollments.Sum(e => e.TotalAmount);
        var totalAmountPaid = enrollments.Sum(e => e.PaidAmount);
        var totalRemainingAmount = totalAmountOwed - totalAmountPaid;

        var activeEnrollments = enrollments.Count(e => e.EnrollmentStatus == EnrollmentStatus.Active);
        var completedEnrollments = enrollments.Count(e => e.EnrollmentStatus == EnrollmentStatus.Completed);

        // تبدیل ثبت‌نام‌ها به DTO
        var enrollmentDtos = enrollments.Select(e => new StudentEnrollmentSummaryDto
        {
            CourseId = e.CourseId,
            CourseName = e.Course.Title,
            EnrollmentDate = e.EnrollmentDate,
            TotalAmount = e.TotalAmount,
            PaidAmount = e.PaidAmount,
            RemainingAmount = e.GetRemainingAmount(),
            PaymentStatus = e.PaymentStatus.ToString(),
            PaymentStatusDisplay = GetPaymentStatusDisplay(e.PaymentStatus),
            EnrollmentStatus = e.EnrollmentStatus.ToString(),
            EnrollmentStatusDisplay = GetEnrollmentStatusDisplay(e.EnrollmentStatus),
            PaymentPercentage = e.GetPaymentPercentage(),
            IsInstallmentAllowed = e.IsInstallmentAllowed,
            InstallmentCount = e.InstallmentCount
        }).ToList();

        var profile = new StudentProfileDto
        {
            StudentId = student.Id,
            FullName = student.FullName ?? student.UserName ?? "نامشخص",
            Email = student.Email ?? "",
            Mobile = student.Mobile ?? "",
            RegisterDate = student.CreatedAt,
            TotalEnrollments = enrollments.Count,
            ActiveEnrollments = activeEnrollments,
            CompletedEnrollments = completedEnrollments,
            TotalAmountOwed = totalAmountOwed,
            TotalAmountPaid = totalAmountPaid,
            TotalRemainingAmount = totalRemainingAmount,
            Enrollments = enrollmentDtos
        };

        return profile;
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
}