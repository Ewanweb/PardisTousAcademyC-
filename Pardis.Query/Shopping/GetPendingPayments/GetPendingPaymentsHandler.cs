using MediatR;
using AutoMapper;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;
using System.Text.Json;

namespace Pardis.Query.Shopping.GetPendingPayments;

/// <summary>
/// پردازشگر کوئری دریافت پرداخت‌های در انتظار تایید ادمین
/// </summary>
public class GetPendingPaymentsHandler : IRequestHandler<GetPendingPaymentsQuery, List<GetPendingPaymentsResult>>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IMapper _mapper;

    public GetPendingPaymentsHandler(IPaymentAttemptRepository paymentAttemptRepository, IMapper mapper)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
    }

    public async Task<List<GetPendingPaymentsResult>> Handle(GetPendingPaymentsQuery request, CancellationToken cancellationToken)
    {
        var pendingPayments = await _paymentAttemptRepository.GetPendingAdminApprovalAsync(cancellationToken);
        
        var results = new List<GetPendingPaymentsResult>();

        foreach (var payment in pendingPayments)
        {
            var courses = DeserializeCartSnapshot(payment.Order.CartSnapshot);
            var daysWaiting = payment.ReceiptUploadedAt.HasValue 
                ? (DateTime.UtcNow - payment.ReceiptUploadedAt.Value).Days 
                : 0;

            var result = new GetPendingPaymentsResult
            {
                PaymentAttemptId = payment.Id,
                TrackingCode = payment.TrackingCode ?? string.Empty,
                OrderId = payment.OrderId,
                OrderNumber = payment.Order.OrderNumber,
                UserId = payment.UserId,
                UserName = payment.User?.UserName ?? "نامشخص",
                UserEmail = payment.User?.Email ?? string.Empty,
                Amount = payment.Amount,
                Method = payment.Method,
                MethodText = GetPaymentMethodText(payment.Method),
                Status = payment.Status,
                StatusText = GetPaymentStatusText(payment.Status),
                ReceiptImageUrl = payment.ReceiptImageUrl,
                ReceiptFileName = payment.ReceiptFileName,
                ReceiptUploadedAt = payment.ReceiptUploadedAt,
                CreatedAt = payment.CreatedAt,
                DaysWaiting = daysWaiting,
                Courses = courses
            };

            results.Add(result);
        }

        return results.OrderBy(r => r.ReceiptUploadedAt).ToList();
    }

    private List<OrderCourseDto> DeserializeCartSnapshot(string cartSnapshot)
    {
        try
        {
            var items = JsonSerializer.Deserialize<List<dynamic>>(cartSnapshot);
            return items?.Select(item => new OrderCourseDto
            {
                CourseId = Guid.Parse(item.GetProperty("CourseId").GetString() ?? string.Empty),
                Title = item.GetProperty("Title").GetString() ?? string.Empty,
                Thumbnail = item.TryGetProperty("Thumbnail", out JsonElement thumb) ? thumb.GetString() : null,
                Price = item.GetProperty("Price").GetInt64()
            }).ToList() ?? new List<OrderCourseDto>();
        }
        catch
        {
            return new List<OrderCourseDto>();
        }
    }

    private string GetPaymentMethodText(PaymentMethod method) => method switch
    {
        PaymentMethod.Online => "پرداخت آنلاین",
        PaymentMethod.Wallet => "کیف پول",
        PaymentMethod.Manual => "کارت به کارت",
        PaymentMethod.Cash => "نقدی",
        PaymentMethod.Free => "رایگان",
        _ => "نامشخص"
    };

    private string GetPaymentStatusText(PaymentAttemptStatus status) => status switch
    {
        PaymentAttemptStatus.Draft => "پیش‌نویس",
        PaymentAttemptStatus.PendingPayment => "در انتظار پرداخت",
        PaymentAttemptStatus.AwaitingReceiptUpload => "در انتظار آپلود رسید",
        PaymentAttemptStatus.AwaitingAdminApproval => "در انتظار تایید ادمین",
        PaymentAttemptStatus.Paid => "پرداخت شده",
        PaymentAttemptStatus.Failed => "ناموفق",
        PaymentAttemptStatus.Expired => "منقضی شده",
        PaymentAttemptStatus.Refunded => "بازگشت داده شده",
        _ => "نامشخص"
    };
}