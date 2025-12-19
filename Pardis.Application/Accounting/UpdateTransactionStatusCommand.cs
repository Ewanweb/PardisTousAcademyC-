using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Command برای بروزرسانی وضعیت تراکنش
/// </summary>
public class UpdateTransactionStatusCommand : IRequest<OperationResult>
{
    public Guid TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? Description { get; set; }
}