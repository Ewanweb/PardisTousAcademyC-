using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;

public class AdminReviewPaymentCommand : IRequest<OperationResult<AdminReviewPaymentResult>>
{
    public Guid PaymentAttemptId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }
    public string? AdminNote { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty; // CRITICAL: Enforce idempotency
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class AdminReviewPaymentResult
{
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string AdminDecision { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> EnrollmentResults { get; set; } = new();
}
