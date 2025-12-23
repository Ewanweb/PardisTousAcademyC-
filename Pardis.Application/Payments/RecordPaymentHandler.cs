using MediatR;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Application.Payments;

/// <summary>
/// Handler برای ثبت پرداخت جدید
/// </summary>
public class RecordPaymentHandler : IRequestHandler<RecordPaymentCommand, PaymentDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;

    public RecordPaymentHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<PaymentDto> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
        if (enrollment == null)
            throw new ArgumentException("ثبت‌نام یافت نشد", nameof(request.EnrollmentId));

        var recordedByUser = await _userRepository.GetByIdAsync(request.RecordedByUserId);
        if (recordedByUser == null)
            throw new ArgumentException("کاربر ثبت‌کننده یافت نشد", nameof(request.RecordedByUserId));

        // تبدیل string به enum
        if (!Enum.TryParse<EnrollmentPaymentMethod>(request.PaymentMethod, true, out var paymentMethodEnum))
            throw new ArgumentException("روش پرداخت نامعتبر است", nameof(request.PaymentMethod));

        // اضافه کردن پرداخت به ثبت‌نام
        enrollment.AddPayment(request.Amount, request.Description ?? string.Empty, paymentMethodEnum);

        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = Guid.NewGuid(), // برای سازگاری با DTO
            EnrollmentId = enrollment.Id,
            Amount = request.Amount,
            PaymentMethod = paymentMethodEnum,
            PaymentMethodDisplay = GetPaymentMethodDisplay(paymentMethodEnum),
            Description = request.Description,
            PaymentDate = request.PaymentDate,
            Status = enrollment.PaymentStatus,
            StatusDisplay = GetPaymentStatusDisplay(enrollment.PaymentStatus),
            RecordedByUserId = request.RecordedByUserId,
            RecordedByUserName = recordedByUser.FullName ?? recordedByUser.UserName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static string GetPaymentMethodDisplay(EnrollmentPaymentMethod method)
    {
        return method switch
        {
            EnrollmentPaymentMethod.Online => "پرداخت آنلاین",
            EnrollmentPaymentMethod.Cash => "نقدی",
            EnrollmentPaymentMethod.Transfer => "انتقال بانکی",
            EnrollmentPaymentMethod.Cheque => "چک",
            _ => method.ToString()
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