using MediatR;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Query برای دریافت جزئیات یک تراکنش
/// </summary>
public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto?>;