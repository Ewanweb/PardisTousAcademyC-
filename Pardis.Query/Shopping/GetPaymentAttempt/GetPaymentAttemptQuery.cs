using MediatR;
namespace Pardis.Query.Shopping.GetPaymentAttempt;

/// <summary>
/// Query to get payment attempt details by ID
/// </summary>
public class GetPaymentAttemptQuery : IRequest<GetPaymentAttemptResult?>
{
    /// <summary>
    /// Payment attempt ID
    /// </summary>
    public Guid PaymentAttemptId { get; set; }

    /// <summary>
    /// User ID for authorization (optional)
    /// </summary>
    public string? UserId { get; set; }

    // Backward-compatible alias for older call sites
    public Guid Id
    {
        get => PaymentAttemptId;
        set => PaymentAttemptId = value;
    }

    public GetPaymentAttemptQuery()
    {
    }

    public GetPaymentAttemptQuery(Guid id, string? userId = null)
    {
        PaymentAttemptId = id;
        UserId = userId;
    }
}
