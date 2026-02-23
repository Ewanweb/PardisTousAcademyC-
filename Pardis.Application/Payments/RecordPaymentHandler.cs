using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Payments;

namespace Pardis.Application.Payments;

/// <summary>
/// Handler برای ثبت پرداخت دستی
/// </summary>
public class RecordPaymentHandler : IRequestHandler<RecordPaymentCommand, OperationResult<bool>>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;

    public RecordPaymentHandler(ICourseEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<OperationResult<bool>> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت ثبت‌نام
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);
            if (enrollment == null)
            {
                return OperationResult<bool>.NotFound("ثبت‌نام یافت نشد");
            }

            // بررسی مبلغ
            if (request.Amount <= 0)
            {
                return OperationResult<bool>.Error("مبلغ پرداخت باید بیشتر از صفر باشد");
            }

            var remainingAmount = enrollment.GetRemainingAmount();
            if (request.Amount > remainingAmount)
            {
                return OperationResult<bool>.Error($"مبلغ پرداخت نمی‌تواند بیشتر از مبلغ باقی‌مانده ({remainingAmount:N0} تومان) باشد");
            }

            // ثبت پرداخت
            enrollment.AddPayment(request.Amount, request.PaymentReference, request.Method);

            // ذخیره تغییرات
            await _enrollmentRepository.UpdateAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return OperationResult<bool>.Error(ex.Message);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Error($"خطا در ثبت پرداخت: {ex.Message}");
        }
    }
}
