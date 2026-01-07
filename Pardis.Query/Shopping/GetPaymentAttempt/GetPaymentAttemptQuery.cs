using MediatR;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetPaymentAttempt;

/// <summary>
/// کوئری دریافت جزئیات یک تلاش پرداخت خاص
/// </summary>
public class GetPaymentAttemptQuery : IRequest<GetPaymentAttemptResult>
{
    public Guid PaymentAttemptId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه دریافت جزئیات تلاش پرداخت
/// </summary>
public class GetPaymentAttemptResult
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public long Amount { get; set; }
    public PaymentAttemptStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public PaymentMethod Method { get; set; }
    public string MethodText { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public string? RejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool RequiresReceiptUpload { get; set; }
    
    // خلاصه دوره (برای نمایش در صفحه پرداخت)
    public List<PaymentCourseDto> Courses { get; set; } = new();
}

public class PaymentCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
}
