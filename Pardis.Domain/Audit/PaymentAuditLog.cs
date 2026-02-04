using Pardis.Domain.Users;

namespace Pardis.Domain.Audit;

/// <summary>
/// Immutable audit log for payment operations
/// </summary>
public class PaymentAuditLog : BaseEntity
{
    public Guid PaymentAttemptId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string? AdminUserId { get; private set; }
    public PaymentAuditAction Action { get; private set; }
    public string PreviousStatus { get; private set; } = string.Empty;
    public string NewStatus { get; private set; } = string.Empty;
    public long Amount { get; private set; }
    public string? Reason { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string AdditionalData { get; private set; } = string.Empty; // JSON

    // Navigation Properties
    public User User { get; private set; } = null!;
    public User? AdminUser { get; private set; }

    // Private constructor for EF Core
    private PaymentAuditLog() { }

    public PaymentAuditLog(
        Guid paymentAttemptId,
        string userId,
        PaymentAuditAction action,
        string previousStatus,
        string newStatus,
        long amount,
        string ipAddress,
        string userAgent,
        string? adminUserId = null,
        string? reason = null,
        string? idempotencyKey = null,
        string? additionalData = null)
    {
        Id = Guid.NewGuid();
        PaymentAttemptId = paymentAttemptId;
        UserId = userId;
        AdminUserId = adminUserId;
        Action = action;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Amount = amount;
        Reason = reason;
        IdempotencyKey = idempotencyKey;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        AdditionalData = additionalData ?? "{}";
        Timestamp = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PaymentAuditAction
{
    Created = 1,
    ReceiptUploaded = 2,
    AdminApproved = 3,
    AdminRejected = 4,
    EnrollmentCreated = 5,
    EnrollmentFailed = 6,
    OrderCompleted = 7,
    CartCleared = 8
}