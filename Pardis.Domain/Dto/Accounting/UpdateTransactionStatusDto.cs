using Pardis.Domain.Accounting;

namespace Pardis.Domain.Dto.Accounting;

/// <summary>
/// DTO برای بروزرسانی وضعیت تراکنش
/// </summary>
public class UpdateTransactionStatusDto
{
    public TransactionStatus Status { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? Description { get; set; }
}