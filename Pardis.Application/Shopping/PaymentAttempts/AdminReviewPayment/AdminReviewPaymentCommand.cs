using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;

/// <summary>
/// دستور بررسی پرداخت توسط ادمین
/// </summary>
public class AdminReviewPaymentCommand : IRequest<OperationResult<AdminReviewPaymentResult>>
{
    public Guid PaymentAttemptId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }
}

/// <summary>
/// نتیجه بررسی پرداخت توسط ادمین
/// </summary>
public class AdminReviewPaymentResult
{
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string AdminDecision { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}