using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Command برای بازگشت وجه تراکنش
/// </summary>
public class RefundTransactionCommand : IRequest<OperationResult>
{
    public Guid TransactionId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long? RefundAmount { get; set; } // اگر null باشد، کل مبلغ بازگردانده می‌شود
    public string AdminUserId { get; set; } = string.Empty; // شناسه ادمین که بازگشت وجه را انجام می‌دهد
}