using MediatR;
using AutoMapper;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Shopping;
using System.Text.Json;

namespace Pardis.Query.Shopping.GetMyOrders;

/// <summary>
/// پردازشگر کوئری دریافت سفارش‌های کاربر جاری
/// </summary>
public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, List<GetMyOrdersResult>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetMyOrdersHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<GetMyOrdersResult>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        var results = new List<GetMyOrdersResult>();

        foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
        {
            var courses = DeserializeCartSnapshot(order.CartSnapshot);
            
            var result = new GetMyOrdersResult
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                Status = order.Status,
                StatusText = GetOrderStatusText(order.Status),
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                CourseCount = courses.Count,
                Courses = courses,
                PaymentAttempts = order.PaymentAttempts.Select(pa => new PaymentAttemptSummaryDto
                {
                    PaymentAttemptId = pa.Id,
                    TrackingCode = pa.TrackingCode ?? string.Empty,
                    Method = pa.Method,
                    MethodText = GetPaymentMethodText(pa.Method),
                    Amount = pa.Amount,
                    Status = pa.Status,
                    StatusText = GetPaymentStatusText(pa.Status),
                    CreatedAt = pa.CreatedAt,
                    RequiresAction = pa.RequiresReceiptUpload() || pa.RequiresAdminApproval()
                }).OrderByDescending(pa => pa.CreatedAt).ToList()
            };

            results.Add(result);
        }

        return results;
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

    private string GetOrderStatusText(OrderStatus status) => status switch
    {
        OrderStatus.Draft => "پیش‌نویس",
        OrderStatus.PendingPayment => "در انتظار پرداخت",
        OrderStatus.Completed => "تکمیل شده",
        OrderStatus.Cancelled => "لغو شده",
        _ => "نامشخص"
    };

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