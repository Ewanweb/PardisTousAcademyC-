using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Command برای ایجاد تراکنش جدید
/// </summary>
public class CreateTransactionCommand : IRequest<OperationResult<TransactionDto>>
{
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public long Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Gateway { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? Description { get; set; }
}