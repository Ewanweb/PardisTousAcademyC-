using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Payments;

/// <summary>
/// Handler برای دریافت اقساط دانشجو
/// </summary>
public class GetStudentEnrollmentsHandler : IRequestHandler<GetStudentEnrollmentsQuery, List<CourseEnrollmentDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;

    public GetStudentEnrollmentsHandler(
        IUserRepository userRepository,
        ICourseEnrollmentRepository enrollmentRepository)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<CourseEnrollmentDto>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            return new List<CourseEnrollmentDto>();

        var enrollments = await _enrollmentRepository.GetEnrollmentsWithInstallmentsAsync(request.StudentId, cancellationToken);

        var result = enrollments.Select(enrollment => new CourseEnrollmentDto
        {
            Id = enrollment.Id,
            CourseId = enrollment.CourseId,
            CourseName = enrollment.Course.Title,
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student.FullName ?? enrollment.Student.UserName ?? "نامشخص",
            TotalAmount = enrollment.TotalAmount,
            PaidAmount = enrollment.PaidAmount,
            RemainingAmount = enrollment.GetRemainingAmount(),
            PaymentStatus = enrollment.PaymentStatus,
            PaymentStatusDisplay = GetPaymentStatusDisplay(enrollment.PaymentStatus),
            EnrollmentStatus = enrollment.EnrollmentStatus,
            EnrollmentStatusDisplay = GetEnrollmentStatusDisplay(enrollment.EnrollmentStatus),
            EnrollmentDate = enrollment.EnrollmentDate,
            PaymentPercentage = enrollment.GetPaymentPercentage(),
            IsInstallmentAllowed = enrollment.IsInstallmentAllowed,
            InstallmentCount = enrollment.InstallmentCount,
            Installments = enrollment.InstallmentPayments.Select(installment => new InstallmentPaymentDto
            {
                Id = installment.Id,
                InstallmentNumber = installment.InstallmentNumber,
                Amount = installment.Amount,
                PaidAmount = installment.PaidAmount,
                RemainingAmount = installment.GetRemainingAmount(),
                DueDate = installment.DueDate,
                PaidDate = installment.PaidDate,
                Status = installment.Status,
                StatusDisplay = GetInstallmentStatusDisplay(installment.Status),
                PaymentMethod = installment.PaymentMethod,
                PaymentMethodDisplay = installment.PaymentMethod.HasValue ? GetEnrollmentPaymentMethodDisplay(installment.PaymentMethod.Value) : null,
                PaymentReference = installment.PaymentReference,
                Notes = installment.Notes,
                DaysUntilDue = installment.GetDaysUntilDue(),
                DaysOverdue = installment.GetDaysOverdue()
            }).ToList()
        }).ToList();

        return result;
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

    private static string GetInstallmentStatusDisplay(InstallmentStatus status)
    {
        return status switch
        {
            InstallmentStatus.Unpaid => "پرداخت نشده",
            InstallmentStatus.Partial => "پرداخت جزئی",
            InstallmentStatus.Paid => "پرداخت شده",
            InstallmentStatus.Overdue => "معوق",
            _ => status.ToString()
        };
    }

    private static string GetEnrollmentPaymentMethodDisplay(EnrollmentPaymentMethod method)
    {
        return method switch
        {
            EnrollmentPaymentMethod.Online => "آنلاین",
            EnrollmentPaymentMethod.Cash => "نقدی",
            EnrollmentPaymentMethod.Transfer => "انتقال بانکی",
            EnrollmentPaymentMethod.Cheque => "چک",
            _ => method.ToString()
        };
    }
}