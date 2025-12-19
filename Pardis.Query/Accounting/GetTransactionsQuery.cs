using MediatR;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Query برای دریافت لیست تراکنش‌ها با فیلتر
/// </summary>
public class GetTransactionsQuery : IRequest<(List<TransactionDto> Transactions, int TotalCount)>
{
    public TransactionStatus? Status { get; set; }
    public PaymentMethod? Method { get; set; }
    public string? Gateway { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}