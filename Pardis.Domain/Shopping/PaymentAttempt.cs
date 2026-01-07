using Pardis.Domain.Users;

namespace Pardis.Domain.Shopping;

/// <summary>
/// موجودیت تلاش پرداخت
/// </summary>
public class PaymentAttempt : BaseEntity
{
    public Guid OrderId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public PaymentMethod Method { get; private set; }
    public long Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }
    public string? TrackingCode { get; private set; } // کد پیگیری
    public string? ReceiptImageUrl { get; private set; } // آدرس تصویر رسید
    public string? ReceiptFileName { get; private set; } // نام فایل رسید
    public DateTime? ReceiptUploadedAt { get; private set; } // تاریخ آپلود رسید
    public string? AdminReviewedBy { get; private set; } // شناسه ادمین بررسی‌کننده
    public DateTime? AdminReviewedAt { get; private set; } // تاریخ بررسی ادمین
    public string? AdminDecision { get; private set; } // تصمیم ادمین (Approved/Rejected)
    public string? FailureReason { get; private set; } // دلیل شکست

    // Navigation Properties
    public Order Order { get; private set; } = null!;
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    private PaymentAttempt() { }

    public PaymentAttempt(Guid orderId, string userId, long amount)
    {
        if (amount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید مثبت باشد", nameof(amount));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("شناسه کاربر نمی‌تواند خالی باشد", nameof(userId));

        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        Method = PaymentMethod.Manual; // تنها روش پرداخت مجاز
        Amount = amount;
        Status = PaymentAttemptStatus.Draft;
        TrackingCode = GenerateTrackingCode();
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartPayment()
    {
        if (Status != PaymentAttemptStatus.Draft)
            throw new InvalidOperationException("فقط تلاش‌های پیش‌نویس قابل شروع هستند");

        Status = PaymentAttemptStatus.PendingPayment;
        UpdatedAt = DateTime.UtcNow;
    }

    // Remove unused methods for simplified manual-only payment system
    // MarkAsPaid, MarkAsFailed, MarkAsExpired, IsExpired, CanBeRetried, SetExpiryBasedOnMethod
    // are no longer needed for the simplified manual payment flow

    public void UploadReceipt(string receiptImageUrl, string receiptFileName)
    {
        if (Status != PaymentAttemptStatus.PendingPayment && Status != PaymentAttemptStatus.Failed)
            throw new InvalidOperationException("وضعیت پرداخت برای آپلود رسید مناسب نیست");

        ReceiptImageUrl = receiptImageUrl;
        ReceiptFileName = receiptFileName;
        ReceiptUploadedAt = DateTime.UtcNow;
        Status = PaymentAttemptStatus.AwaitingAdminApproval;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApproveByAdmin(string adminUserId)
    {
        if (Status != PaymentAttemptStatus.AwaitingAdminApproval)
            throw new InvalidOperationException("فقط پرداخت‌های در انتظار تایید قابل تایید هستند");

        Status = PaymentAttemptStatus.Paid;
        AdminReviewedBy = adminUserId;
        AdminReviewedAt = DateTime.UtcNow;
        AdminDecision = "Approved";
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectByAdmin(string adminUserId, string reason)
    {
        if (Status != PaymentAttemptStatus.AwaitingAdminApproval)
            throw new InvalidOperationException("فقط پرداخت‌های در انتظار تایید قابل رد هستند");

        Status = PaymentAttemptStatus.Failed;
        AdminReviewedBy = adminUserId;
        AdminReviewedAt = DateTime.UtcNow;
        AdminDecision = "Rejected";
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    // Remove unused status transition methods and expiry logic
    // These are not needed for the simplified manual payment system

    public bool RequiresReceiptUpload() => Status == PaymentAttemptStatus.PendingPayment || Status == PaymentAttemptStatus.Failed;

    public bool RequiresAdminApproval() => Status == PaymentAttemptStatus.AwaitingAdminApproval;

    private string GenerateTrackingCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100000, 999999);
        return $"PAY-{timestamp}-{random}";
    }

    // Remove expiry logic - not needed for manual payments
}

/// <summary>
/// وضعیت تلاش پرداخت - ساده شده برای پرداخت دستی
/// </summary>
public enum PaymentAttemptStatus
{
    Draft = 0,                      // پیش‌نویس (ایجاد شده اما شروع نشده)
    PendingPayment = 1,             // در انتظار پرداخت (شروع شده)
    AwaitingAdminApproval = 3,      // در انتظار تایید ادمین (رسید آپلود شده)
    Paid = 4,                       // پرداخت شده (موفق)
    Failed = 5                      // ناموفق
}