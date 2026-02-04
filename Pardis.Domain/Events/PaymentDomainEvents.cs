using MediatR;

namespace Pardis.Domain.Events;

/// <summary>
/// Domain events for payment operations
/// </summary>
public abstract class PaymentDomainEvent : INotification
{
    public Guid PaymentAttemptId { get; }
    public string UserId { get; }
    public DateTime OccurredAt { get; }

    protected PaymentDomainEvent(Guid paymentAttemptId, string userId)
    {
        PaymentAttemptId = paymentAttemptId;
        UserId = userId;
        OccurredAt = DateTime.UtcNow;
    }
}

public class PaymentApprovedEvent : PaymentDomainEvent
{
    public string AdminUserId { get; }
    public long Amount { get; }
    public Guid OrderId { get; }

    public PaymentApprovedEvent(Guid paymentAttemptId, string userId, string adminUserId, long amount, Guid orderId)
        : base(paymentAttemptId, userId)
    {
        AdminUserId = adminUserId;
        Amount = amount;
        OrderId = orderId;
    }
}

public class PaymentRejectedEvent : PaymentDomainEvent
{
    public string AdminUserId { get; }
    public string Reason { get; }

    public PaymentRejectedEvent(Guid paymentAttemptId, string userId, string adminUserId, string reason)
        : base(paymentAttemptId, userId)
    {
        AdminUserId = adminUserId;
        Reason = reason;
    }
}

public class ReceiptUploadedEvent : PaymentDomainEvent
{
    public string ReceiptUrl { get; }
    public string FileName { get; }

    public ReceiptUploadedEvent(Guid paymentAttemptId, string userId, string receiptUrl, string fileName)
        : base(paymentAttemptId, userId)
    {
        ReceiptUrl = receiptUrl;
        FileName = fileName;
    }
}

public class EnrollmentCreatedEvent : PaymentDomainEvent
{
    public Guid CourseId { get; }
    public Guid EnrollmentId { get; }
    public long Price { get; }

    public EnrollmentCreatedEvent(Guid paymentAttemptId, string userId, Guid courseId, Guid enrollmentId, long price)
        : base(paymentAttemptId, userId)
    {
        CourseId = courseId;
        EnrollmentId = enrollmentId;
        Price = price;
    }
}

public class EnrollmentFailedEvent : PaymentDomainEvent
{
    public Guid CourseId { get; }
    public string ErrorMessage { get; }

    public EnrollmentFailedEvent(Guid paymentAttemptId, string userId, Guid courseId, string errorMessage)
        : base(paymentAttemptId, userId)
    {
        CourseId = courseId;
        ErrorMessage = errorMessage;
    }
}