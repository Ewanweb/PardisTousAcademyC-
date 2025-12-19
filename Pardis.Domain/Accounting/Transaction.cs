using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Domain.Accounting;

/// <summary>
/// موجودیت تراکنش مالی
/// </summary>
public class Transaction : BaseEntity
{
    public string TransactionId { get; set; } = string.Empty; // شناسه یکتا تراکنش
    public string UserId { get; set; } = string.Empty; // شناسه کاربر
    public Guid CourseId { get; set; } // شناسه دوره
    public long Amount { get; set; } // مبلغ به ریال
    public TransactionStatus Status { get; set; } // وضعیت تراکنش
    public PaymentMethod Method { get; set; } // روش پرداخت
    public string? Gateway { get; set; } // درگاه پرداخت
    public string? GatewayTransactionId { get; set; } // شناسه تراکنش در درگاه
    public string? Description { get; set; } // توضیحات
    public string? RefundReason { get; set; } // دلیل بازگشت وجه
    public DateTime? RefundedAt { get; set; } // تاریخ بازگشت وجه
    public long RefundAmount { get; set; } = 0; // مبلغ بازگشتی

    // Navigation Properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}

/// <summary>
/// وضعیت تراکنش
/// </summary>
public enum TransactionStatus
{
    Pending = 0,    // در انتظار
    Completed = 1,  // تکمیل شده
    Failed = 2,     // ناموفق
    Refunded = 3,   // بازگشت وجه
    Cancelled = 4   // لغو شده
}

/// <summary>
/// روش پرداخت
/// </summary>
public enum PaymentMethod
{
    Online = 0,     // آنلاین
    Wallet = 1,     // کیف پول
    Cash = 2,       // نقدی
    Transfer = 3    // انتقال بانکی
}