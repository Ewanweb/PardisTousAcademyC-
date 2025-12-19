using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Dto.Students;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Students;

/// <summary>
/// Handler برای دریافت خلاصه مالی دانشجو
/// </summary>
public class GetStudentFinancialSummaryHandler : IRequestHandler<GetStudentFinancialSummaryQuery, StudentFinancialSummaryDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IRepository<InstallmentPayment> _installmentRepository;

    public GetStudentFinancialSummaryHandler(
        IUserRepository userRepository, 
        ICourseEnrollmentRepository enrollmentRepository,
        IRepository<InstallmentPayment> installmentRepository)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _installmentRepository = installmentRepository;
    }

    public async Task<StudentFinancialSummaryDto?> Handle(GetStudentFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);

        if (student == null)
            return null;

        // دریافت اطلاعات ثبت‌نام‌ها
        var enrollments = await _enrollmentRepository.GetEnrollmentsWithInstallmentsAsync(request.StudentId, cancellationToken);

        if (!enrollments.Any())
        {
            return new StudentFinancialSummaryDto
            {
                StudentId = request.StudentId,
                StudentName = student.FullName ?? student.UserName ?? "نامشخص"
            };
        }

        // محاسبه آمارهای مالی
        var totalAmountOwed = enrollments.Sum(e => e.TotalAmount);
        var totalAmountPaid = enrollments.Sum(e => e.PaidAmount);
        var totalRemainingAmount = totalAmountOwed - totalAmountPaid;

        var paidEnrollments = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Paid);
        var partialPaidEnrollments = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Partial);
        var unpaidEnrollments = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Unpaid);

        // محاسبه اقساط معوق
        var enrollmentIds = enrollments.Select(e => e.Id).ToList();
        var overdueInstallments = await _installmentRepository.Table
            .Where(ip => enrollmentIds.Contains(ip.EnrollmentId) &&
                        ip.Status != InstallmentStatus.Paid &&
                        ip.DueDate < DateTime.UtcNow)
            .CountAsync(cancellationToken);

        var paymentPercentage = totalAmountOwed > 0 ? (decimal)totalAmountPaid / totalAmountOwed * 100 : 0;

        var summary = new StudentFinancialSummaryDto
        {
            StudentId = request.StudentId,
            StudentName = student.FullName ?? student.UserName ?? "نامشخص",
            TotalEnrollments = enrollments.Count,
            TotalAmountOwed = totalAmountOwed,
            TotalAmountPaid = totalAmountPaid,
            TotalRemainingAmount = totalRemainingAmount,
            PaidEnrollments = paidEnrollments,
            PartialPaidEnrollments = partialPaidEnrollments,
            UnpaidEnrollments = unpaidEnrollments,
            OverdueInstallments = overdueInstallments,
            PaymentPercentage = paymentPercentage
        };

        return summary;
    }
}