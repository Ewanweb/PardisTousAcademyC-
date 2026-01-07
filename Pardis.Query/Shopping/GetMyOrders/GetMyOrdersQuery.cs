using MediatR;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetMyOrders;

/// <summary>
/// کوئری دریافت سفارش‌های کاربر جاری
/// </summary>
public class GetMyOrdersQuery : IRequest<List<GetMyOrdersResult>>
{
    public string UserId { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// نتیجه دریافت سفارش‌های کاربر
/// </summary>
public class GetMyOrdersResult
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public string Currency { get; set; } = "IRR";
    public OrderStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int CourseCount { get; set; }
    public List<OrderCourseDto> Courses { get; set; } = new();
    public List<PaymentAttemptSummaryDto> PaymentAttempts { get; set; } = new();
}

/// <summary>
/// DTO دوره در سفارش
/// </summary>
public class OrderCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public long Price { get; set; }
}

/// <summary>
/// DTO خلاصه تلاش پرداخت
/// </summary>
public class PaymentAttemptSummaryDto
{
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public PaymentMethod Method { get; set; }
    public string MethodText { get; set; } = string.Empty;
    public long Amount { get; set; }
    public PaymentAttemptStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public string? RejectReason { get; set; }
    public string? AdminDecision { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool RequiresAction { get; set; }
}