using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Handler برای دریافت جزئیات یک تراکنش
/// </summary>
public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public GetTransactionByIdHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.Table
            .Include(t => t.User)
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

        if (transaction == null)
            return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            TransactionId = transaction.TransactionId,
            StudentName = transaction.User.FullName ?? transaction.User.UserName ?? "نامشخص",
            StudentEmail = transaction.User.Email ?? "",
            CourseName = transaction.Course.Title,
            Amount = transaction.Amount,
            Status = transaction.Status.ToString(),
            StatusDisplay = GetStatusDisplay(transaction.Status),
            Method = transaction.Method.ToString(),
            MethodDisplay = GetMethodDisplay(transaction.Method),
            Gateway = transaction.Gateway,
            GatewayTransactionId = transaction.GatewayTransactionId,
            Description = transaction.Description,
            RefundReason = transaction.RefundReason,
            RefundedAt = transaction.RefundedAt,
            RefundAmount = transaction.RefundAmount,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }

    private static string GetStatusDisplay(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Pending => "در انتظار",
            TransactionStatus.Completed => "تکمیل شده",
            TransactionStatus.Failed => "ناموفق",
            TransactionStatus.Refunded => "بازگشت وجه",
            TransactionStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }

    private static string GetMethodDisplay(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Online => "آنلاین",
            PaymentMethod.Wallet => "کیف پول",
            PaymentMethod.Cash => "نقدی",
            PaymentMethod.Transfer => "انتقال بانکی",
            _ => method.ToString()
        };
    }
}