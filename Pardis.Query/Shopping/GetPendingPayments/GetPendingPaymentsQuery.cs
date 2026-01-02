using MediatR;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetPendingPayments;

/// <summary>
/// کوئری دریافت پرداخت‌های در انتظار تایید ادمین
/// </summary>
public class GetPendingPaymentsQuery : IRequest<List<GetPendingPaymentsResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// نتیجه دریافت پرداخت‌های در انتظار تایید
/// </summary>
public class GetPendingPaymentsResult
{
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public long Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string MethodText { get; set; } = string.Empty;
    public PaymentAttemptStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string? ReceiptImageUrl { get; set; }
    public string? ReceiptFileName { get; set; }
    public DateTime? ReceiptUploadedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DaysWaiting { get; set; }
    public List<OrderCourseDto> Courses { get; set; } = new();
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