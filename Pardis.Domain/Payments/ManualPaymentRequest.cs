using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Domain.Payments;

/// <summary>
/// موجودیت درخواست پرداخت دستی
/// </summary>
public class ManualPaymentRequest : BaseEntity
{
    public Guid CourseId { get; private set; }
    public string StudentId { get; private set; } = string.Empty;
    public long Amount { get; private set; }
    public ManualPaymentStatus Status { get; private set; }
    public string? ReceiptFileUrl { get; private set; }
    public string? ReceiptFileName { get; private set; }
    public DateTime? ReceiptUploadedAt { get; private set; }
    public string? AdminReviewedBy { get; private set; }
    public DateTime? AdminReviewedAt { get; private set; }
    public string? RejectReason { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public User Student { get; private set; } = null!;
    public User? AdminReviewer { get; private set; }

    // Private constructor for EF Core
    private ManualPaymentRequest() { }

    public ManualPaymentRequest(Guid courseId, string studentId, long amount)
    {
        if (amount <= 0)
            throw new ArgumentException("مبلغ باید مثبت باشد", nameof(amount));

        CourseId = courseId;
        StudentId = studentId;
        Amount = amount;
        Status = ManualPaymentStatus.PendingReceipt;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UploadReceipt(string receiptFileUrl, string receiptFileName)
    {
        if (Status != ManualPaymentStatus.PendingReceipt)
            throw new InvalidOperationException("فقط در وضعیت انتظار رسید می‌توان رسید آپلود کرد");

        ReceiptFileUrl = receiptFileUrl;
        ReceiptFileName = receiptFileName;
        ReceiptUploadedAt = DateTime.UtcNow;
        Status = ManualPaymentStatus.PendingApproval;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string adminUserId)
    {
        if (Status != ManualPaymentStatus.PendingApproval)
            throw new InvalidOperationException("فقط درخواست‌های در انتظار تایید قابل تایید هستند");

        Status = ManualPaymentStatus.Approved;
        AdminReviewedBy = adminUserId;
        AdminReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string adminUserId, string reason)
    {
        if (Status != ManualPaymentStatus.PendingApproval)
            throw new InvalidOperationException("فقط درخواست‌های در انتظار تایید قابل رد هستند");

        Status = ManualPaymentStatus.Rejected;
        AdminReviewedBy = adminUserId;
        AdminReviewedAt = DateTime.UtcNow;
        RejectReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AllowResubmission()
    {
        if (Status != ManualPaymentStatus.Rejected)
            throw new InvalidOperationException("فقط درخواست‌های رد شده قابل ارسال مجدد هستند");

        Status = ManualPaymentStatus.PendingReceipt;
        ReceiptFileUrl = null;
        ReceiptFileName = null;
        ReceiptUploadedAt = null;
        RejectReason = null;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// وضعیت درخواست پرداخت دستی
/// </summary>
public enum ManualPaymentStatus
{
    PendingReceipt = 0,    // در انتظار آپلود رسید
    PendingApproval = 1,   // در انتظار تایید ادمین
    Approved = 2,          // تایید شده
    Rejected = 3           // رد شده
}