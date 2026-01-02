using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Payments;
using AutoMapper;

namespace Pardis.Application.Payments.ManualPayment.ReviewManualPayment;

/// <summary>
/// Handler برای تایید/رد پرداخت دستی توسط ادمین
/// </summary>
public class ReviewManualPaymentHandler : IRequestHandler<ReviewManualPaymentCommand, OperationResult<ManualPaymentRequestDto>>
{
    private readonly IManualPaymentRequestRepository _manualPaymentRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;

    public ReviewManualPaymentHandler(
        IManualPaymentRequestRepository manualPaymentRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    {
        _manualPaymentRepository = manualPaymentRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<ManualPaymentRequestDto>> Handle(ReviewManualPaymentCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود درخواست پرداخت
        var paymentRequest = await _manualPaymentRepository.GetByIdWithDetailsAsync(request.PaymentRequestId, cancellationToken);
        if (paymentRequest == null)
            return OperationResult<ManualPaymentRequestDto>.Error("درخواست پرداخت یافت نشد");

        // بررسی وضعیت درخواست
        if (paymentRequest.Status != ManualPaymentStatus.PendingApproval)
            return OperationResult<ManualPaymentRequestDto>.Error("فقط درخواست‌های در انتظار تایید قابل بررسی هستند");

        if (request.IsApproved)
        {
            // تایید پرداخت
            paymentRequest.Approve(request.AdminUserId);

            // ایجاد ثبت‌نام در دوره
            var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(
                paymentRequest.StudentId, 
                paymentRequest.CourseId, 
                cancellationToken);

            if (existingEnrollment == null)
            {
                // ایجاد ثبت‌نام جدید
                var enrollment = new CourseEnrollment(
                    paymentRequest.CourseId,
                    paymentRequest.StudentId,
                    paymentRequest.Amount);

                // ثبت پرداخت کامل
                enrollment.AddPayment(
                    paymentRequest.Amount,
                    $"Manual-{paymentRequest.Id}",
                    EnrollmentPaymentMethod.Transfer);

                await _enrollmentRepository.AddAsync(enrollment);
            }
            else
            {
                // اضافه کردن پرداخت به ثبت‌نام موجود
                existingEnrollment.AddPayment(
                    paymentRequest.Amount,
                    $"Manual-{paymentRequest.Id}",
                    EnrollmentPaymentMethod.Transfer);
            }
        }
        else
        {
            // رد پرداخت
            if (string.IsNullOrWhiteSpace(request.RejectReason))
                return OperationResult<ManualPaymentRequestDto>.Error("دلیل رد الزامی است");

            paymentRequest.Reject(request.AdminUserId, request.RejectReason);
        }

        // ذخیره تغییرات
        await _manualPaymentRepository.SaveChangesAsync(cancellationToken);
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);

        // تبدیل به DTO با استفاده از AutoMapper
        var dto = _mapper.Map<ManualPaymentRequestDto>(paymentRequest);

        var message = request.IsApproved ? "پرداخت با موفقیت تایید شد" : "پرداخت رد شد";
        return OperationResult<ManualPaymentRequestDto>.Success(dto);
    }
}