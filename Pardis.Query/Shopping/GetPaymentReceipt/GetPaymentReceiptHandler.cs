using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetPaymentReceipt;

/// <summary>
/// Handler برای دریافت رسید پرداخت
/// </summary>
public class GetPaymentReceiptHandler : IRequestHandler<GetPaymentReceiptQuery, GetPaymentReceiptResult?>
{
    private readonly IRepository<PaymentAttempt> _paymentAttemptRepository;

    public GetPaymentReceiptHandler(IRepository<PaymentAttempt> paymentAttemptRepository)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
    }

    public async Task<GetPaymentReceiptResult?> Handle(GetPaymentReceiptQuery request, CancellationToken cancellationToken)
    {
        var paymentAttempt = await _paymentAttemptRepository.Table
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentAttemptId && 
                                      p.Order.UserId == request.UserId, 
                                 cancellationToken);

        if (paymentAttempt == null)
            return null;

        return new GetPaymentReceiptResult
        {
            PaymentAttemptId = paymentAttempt.Id,
            ReceiptImageUrl = paymentAttempt.ReceiptImageUrl,
            ReceiptFileName = paymentAttempt.ReceiptImageUrl?.Split('/').LastOrDefault(),
            UploadedAt = paymentAttempt.ReceiptUploadedAt,
            PaymentStatus = GetStatusDisplayText(paymentAttempt.Status),
            Amount = paymentAttempt.Amount,
            TrackingCode = paymentAttempt.TrackingCode
        };
    }

    private static string GetStatusDisplayText(PaymentAttemptStatus status)
    {
        return status switch
        {
            PaymentAttemptStatus.Draft => "پیش‌نویس",
            PaymentAttemptStatus.PendingPayment => "در انتظار آپلود رسید",
            PaymentAttemptStatus.AwaitingAdminApproval => "در انتظار تایید ادمین",
            PaymentAttemptStatus.Paid => "پرداخت شده",
            PaymentAttemptStatus.Failed => "ناموفق",
            _ => "نامشخص"
        };
    }
}