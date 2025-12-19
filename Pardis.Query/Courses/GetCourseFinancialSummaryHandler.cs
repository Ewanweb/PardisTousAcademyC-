using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Payments;
using Pardis.Domain.Courses;

namespace Pardis.Query.Courses;

/// <summary>
/// Handler برای دریافت خلاصه مالی یک دوره
/// </summary>
public class GetCourseFinancialSummaryHandler : IRequestHandler<GetCourseFinancialSummaryQuery, CourseFinancialSummaryDto?>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IRepository<InstallmentPayment> _installmentRepository;

    public GetCourseFinancialSummaryHandler(
        ICourseRepository courseRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        IRepository<InstallmentPayment> installmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _installmentRepository = installmentRepository;
    }

    public async Task<CourseFinancialSummaryDto?> Handle(GetCourseFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);

        if (course == null)
            return null;

        // دریافت اطلاعات ثبت‌نام‌های این دوره
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(request.CourseId, cancellationToken);

        if (!enrollments.Any())
        {
            return new CourseFinancialSummaryDto
            {
                CourseId = request.CourseId,
                CourseName = course.Title,
                TotalRevenue = 0,
                PendingAmount = 0,
                OverdueAmount = 0,
                TotalStudents = 0,
                PaidStudents = 0,
                PartialPaidStudents = 0,
                UnpaidStudents = 0,
                AveragePaymentProgress = 0,
                OverdueInstallments = 0,
                LastPaymentDate = DateTime.MinValue
            };
        }

        // محاسبه آمارهای مالی
        var totalRevenue = enrollments.Sum(e => e.PaidAmount);
        var pendingAmount = enrollments.Sum(e => e.GetRemainingAmount());
        var totalStudents = enrollments.Count;

        var paidStudents = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Paid);
        var partialPaidStudents = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Partial);
        var unpaidStudents = enrollments.Count(e => e.PaymentStatus == PaymentStatus.Unpaid);

        // محاسبه میانگین پیشرفت پرداخت
        var averagePaymentProgress = enrollments.Any() 
            ? enrollments.Average(e => e.GetPaymentPercentage()) 
            : 0;

        // محاسبه اقساط معوق
        var enrollmentIds = enrollments.Select(e => e.Id).ToList();
        var overdueInstallments = await _installmentRepository.Table
            .Where(ip => enrollmentIds.Contains(ip.EnrollmentId) &&
                        ip.Status != InstallmentStatus.Paid &&
                        ip.DueDate < DateTime.UtcNow)
            .CountAsync(cancellationToken);

        // محاسبه مبلغ معوق
        var overdueAmount = await _installmentRepository.Table
            .Where(ip => enrollmentIds.Contains(ip.EnrollmentId) &&
                        ip.Status != InstallmentStatus.Paid &&
                        ip.DueDate < DateTime.UtcNow)
            .SumAsync(ip => ip.Amount - ip.PaidAmount, cancellationToken);

        // آخرین تاریخ پرداخت
        var lastPaymentDate = await _installmentRepository.Table
            .Where(ip => enrollmentIds.Contains(ip.EnrollmentId) &&
                        ip.PaidDate.HasValue)
            .OrderByDescending(ip => ip.PaidDate)
            .Select(ip => ip.PaidDate!.Value)
            .FirstOrDefaultAsync(cancellationToken);

        var summary = new CourseFinancialSummaryDto
        {
            CourseId = request.CourseId,
            CourseName = course.Title,
            TotalRevenue = totalRevenue,
            PendingAmount = pendingAmount,
            OverdueAmount = overdueAmount,
            TotalStudents = totalStudents,
            PaidStudents = paidStudents,
            PartialPaidStudents = partialPaidStudents,
            UnpaidStudents = unpaidStudents,
            AveragePaymentProgress = averagePaymentProgress,
            OverdueInstallments = overdueInstallments,
            LastPaymentDate = lastPaymentDate
        };

        return summary;
    }
}