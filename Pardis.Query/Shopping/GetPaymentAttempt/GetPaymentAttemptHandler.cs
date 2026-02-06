using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Infrastructure;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Shopping.GetPaymentAttempt;

public class GetPaymentAttemptHandler : IRequestHandler<GetPaymentAttemptQuery, GetPaymentAttemptResult?>
{
    private readonly AppDbContext _context;

    public GetPaymentAttemptHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetPaymentAttemptResult?> Handle(GetPaymentAttemptQuery request, CancellationToken cancellationToken)
    {
        // If UserId is provided, filter by it (for user-specific queries)
        // If UserId is null, don't filter (for admin queries)
        var query = _context.PaymentAttempts
            .Include(p => p.Order)
            .Where(p => p.Id == request.PaymentAttemptId);

        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(p => p.UserId == request.UserId);
        }

        var attempt = await query.FirstOrDefaultAsync(cancellationToken);

        if (attempt == null)
            return null!;

        // Get admin reviewer name if available
        string? adminReviewerName = null;
        if (!string.IsNullOrEmpty(attempt.AdminReviewedBy))
        {
            var adminUser = await _context.Users
                .Where(u => u.Id == attempt.AdminReviewedBy)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
            adminReviewerName = adminUser;
        }

        var result = new GetPaymentAttemptResult
        {
            Id = attempt.Id,
            OrderId = attempt.OrderId,
            OrderNumber = attempt.Order.OrderNumber,
            Amount = attempt.Amount,
            Status = attempt.Status,
            StatusText = GetStatusText(attempt.Status),
            Method = attempt.Method,
            MethodText = GetMethodText(attempt.Method),
            ReceiptUrl = attempt.ReceiptImageUrl,
            RejectReason = attempt.FailureReason,
            CreatedAt = attempt.CreatedAt,
            ExpiresAt = null, // Manual payments don't expire
            RequiresReceiptUpload = attempt.RequiresReceiptUpload(),
            AdminReviewerId = attempt.AdminReviewedBy,
            AdminReviewerName = adminReviewerName,
            AdminReviewedAt = attempt.AdminReviewedAt,
            AdminDecision = attempt.AdminDecision
        };

        return result;
    }

    private string GetStatusText(PaymentAttemptStatus status)
    {
        return status switch
        {
            PaymentAttemptStatus.Draft => "پیش‌نویس",
            PaymentAttemptStatus.PendingPayment => "در انتظار آپلود رسید",
            PaymentAttemptStatus.AwaitingAdminApproval => "در انتظار تایید ادمین",
            PaymentAttemptStatus.Paid => "پرداخت شده",
            PaymentAttemptStatus.Failed => "ناموفق (رد شده)",
            _ => "نامشخص"
        };
    }

    private string GetMethodText(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Manual => "کارت به کارت",
            _ => "نامشخص"
        };
    }
}
