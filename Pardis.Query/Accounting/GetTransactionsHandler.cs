using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Query.Accounting;

/// <summary>
/// Handler برای دریافت لیست تراکنش‌ها با فیلتر
/// </summary>
public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, (List<TransactionDto> Transactions, int TotalCount)>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public GetTransactionsHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<(List<TransactionDto> Transactions, int TotalCount)> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = _transactionRepository.Table
            .Include(t => t.User)
            .Include(t => t.Course)
            .AsQueryable();

        // اعمال فیلترها
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Method.HasValue)
            query = query.Where(t => t.Method == request.Method.Value);

        if (!string.IsNullOrEmpty(request.Gateway))
            query = query.Where(t => t.Gateway == request.Gateway);

        if (request.FromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(t => t.CreatedAt <= request.ToDate.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t => 
                t.User.FullName!.ToLower().Contains(searchTerm) ||
                t.User.Email!.ToLower().Contains(searchTerm) ||
                t.Course.Title.ToLower().Contains(searchTerm) ||
                t.TransactionId.ToLower().Contains(searchTerm));
        }

        // شمارش کل
        var totalCount = await query.CountAsync(cancellationToken);

        // صفحه‌بندی
        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                TransactionId = t.TransactionId,
                StudentName = t.User.FullName ?? t.User.UserName ?? "نامشخص",
                StudentEmail = t.User.Email ?? "",
                CourseName = t.Course.Title,
                Amount = t.Amount,
                Status = t.Status.ToString(),
                StatusDisplay = GetStatusDisplay(t.Status),
                Method = t.Method.ToString(),
                MethodDisplay = GetMethodDisplay(t.Method),
                Gateway = t.Gateway,
                GatewayTransactionId = t.GatewayTransactionId,
                Description = t.Description,
                RefundReason = t.RefundReason,
                RefundedAt = t.RefundedAt,
                RefundAmount = t.RefundAmount,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return (transactions, totalCount);
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