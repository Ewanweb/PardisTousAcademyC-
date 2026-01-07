using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Shopping;

namespace Pardis.Application.Shopping.Checkout.CreateCheckout;

/// <summary>
/// دستور ایجاد checkout (سفارش)
/// </summary>
public class CreateCheckoutCommand : IRequest<OperationResult<CreateCheckoutResult>>
{
    public string UserId { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public string? IdempotencyKey { get; set; }
}

/// <summary>
/// نتیجه ایجاد checkout
/// </summary>
public class CreateCheckoutResult
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentAttemptStatus PaymentStatus { get; set; }
    public bool RequiresReceiptUpload { get; set; }
    public bool IsFreePurchase { get; set; }
    public string Message { get; set; } = string.Empty;
}