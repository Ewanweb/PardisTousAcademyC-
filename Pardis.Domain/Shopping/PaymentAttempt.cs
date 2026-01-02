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
    public string? ProviderReference { get; private set; } // شماره مرجع از درگاه
    public string? TrackingCode { get; private set; } // کد پیگیری
    public string? ReceiptImageUrl { get; private set; } // آدرس تصویر رسید (برای پرداخت دستی)
    public string? ReceiptFileName { get; private set; } // نام فایل رسید
    public DateTime? ReceiptUploadedAt { get; private set; } // تاریخ آپلود رسید
    public string? AdminReviewedBy { get; private set; } // شناسه ادمین بررسی‌کننده
    public DateTime? AdminReviewedAt { get; private set; } // تاریخ بررسی ادمین
    public string? AdminDecision { get; private set; } // تصمیم ادمین (Approved/Rejected)
    public string? FailureReason { get; private set; } // دلیل شکست
    public DateTime? ExpiresAt { get; private set; } // تاریخ انقضا
    public string? GatewayResponse { get; private set; } // پاسخ درگاه (JSON)

    // Navigation Properties
    public Order Order { get; private set; } = null!;
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    private PaymentAttempt() { }

    public PaymentAttempt(Guid orderId, string userId, PaymentMethod method, long amount)
    {
        if (amount <= 0)
            throw new ArgumentException("مبلغ پرداخت باید مثبت باشد", nameof(amount));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("شناسه کاربر نمی‌تواند خالی باشد", nameof(userId));

        OrderId = orderId;
        UserId = userId;
        Method = method;
        Amount = amount;
        Status = PaymentAttemptStatus.Draft;
        TrackingCode = GenerateTrackingCode();
        
        // تنظیم تاریخ انقضا بر اساس روش پرداخت
        SetExpiryBasedOnMethod(method);
        
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

    public void MarkAsPaid(string? providerReference = null, string? gatewayResponse = null)
    {
        if (Status == PaymentAttemptStatus.Paid)
            throw new InvalidOperationException("این پرداخت قبلاً تکمیل شده است");

        Status = PaymentAttemptStatus.Paid;
        ProviderReference = providerReference;
        GatewayResponse = gatewayResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason, string? gatewayResponse = null)
    {
        if (Status == PaymentAttemptStatus.Paid)
            throw new InvalidOperationException("پرداخت موفق قابل تغییر به ناموفق نیست");

        Status = PaymentAttemptStatus.Failed;
        FailureReason = reason;
        GatewayResponse = gatewayResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UploadReceipt(string receiptImageUrl, string receiptFileName)
    {
        if (Method != PaymentMethod.Manual)
            throw new InvalidOperationException("فقط پرداخت‌های دستی می‌توانند رسید داشته باشند");

        if (Status != PaymentAttemptStatus.PendingPayment && Status != PaymentAttemptStatus.AwaitingReceiptUpload)
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

    public void MarkAsExpired()
    {
        if (Status == PaymentAttemptStatus.Paid)
            throw new InvalidOperationException("پرداخت موفق قابل انقضا نیست");

        Status = PaymentAttemptStatus.Expired;
        FailureReason = "Payment attempt expired";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired() => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    public bool CanBeRetried() => Status == PaymentAttemptStatus.Failed || Status == PaymentAttemptStatus.Expired;

    public bool RequiresReceiptUpload() => Method == PaymentMethod.Manual && Status == PaymentAttemptStatus.PendingPayment;

    public bool RequiresAdminApproval() => Status == PaymentAttemptStatus.AwaitingAdminApproval;

    private string GenerateTrackingCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100000, 999999);
        return $"PAY-{timestamp}-{random}";
    }

    private void SetExpiryBasedOnMethod(PaymentMethod method)
    {
        ExpiresAt = method switch
        {
            PaymentMethod.Online => DateTime.UtcNow.AddMinutes(15), // درگاه آنلاین 15 دقیقه
            PaymentMethod.Manual => DateTime.UtcNow.AddDays(3),     // پرداخت دستی 3 روز
            PaymentMethod.Wallet => DateTime.UtcNow.AddMinutes(5),  // کیف پول 5 دقیقه
            PaymentMethod.Free => null,                             // رایگان بدون انقضا
            _ => DateTime.UtcNow.AddHours(1)                        // سایر موارد 1 ساعت
        };
    }
}

/// <summary>
/// وضعیت تلاش پرداخت
/// </summary>
public enum PaymentAttemptStatus
{
    Draft = 0,                      // پیش‌نویس (ایجاد شده اما شروع نشده)
    PendingPayment = 1,             // در انتظار پرداخت (شروع شده)
    AwaitingReceiptUpload = 2,      // در انتظار آپلود رسید (پرداخت دستی)
    AwaitingAdminApproval = 3,      // در انتظار تایید ادمین (رسید آپلود شده)
    Paid = 4,                       // پرداخت شده (موفق)
    Failed = 5,                     // ناموفق
    Expired = 6,                    // منقضی شده
    Refunded = 7                    // بازگشت داده شده
}