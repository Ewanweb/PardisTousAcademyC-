using MediatR;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Query برای دریافت آمار حسابداری
/// </summary>
public record GetAccountingStatsQuery : IRequest<AccountingStatsDto>;