using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using AutoMapper;

namespace Pardis.Application.Payments.AddEnrollmentPayment;

/// <summary>
/// Handler برای اضافه کردن پرداخت به ثبت‌نام
/// </summary>
public class AddEnrollmentPaymentHandler : IRequestHandler<AddEnrollmentPaymentCommand, CourseEnrollmentDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public AddEnrollmentPaymentHandler(ICourseEnrollmentRepository enrollmentRepository, IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<CourseEnrollmentDto> Handle(AddEnrollmentPaymentCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود ثبت‌نام
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
        if (enrollment == null)
            throw new ArgumentException("ثبت‌نام یافت نشد", nameof(request.EnrollmentId));

        // اضافه کردن پرداخت
        enrollment.AddPayment(request.Amount, request.PaymentReference, request.Method);

        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        // بازگشت DTO با استفاده از AutoMapper
        return _mapper.Map<CourseEnrollmentDto>(enrollment);
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