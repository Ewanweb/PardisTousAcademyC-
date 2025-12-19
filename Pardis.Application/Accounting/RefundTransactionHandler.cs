using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Handler برای بازگشت وجه تراکنش
/// </summary>
public class RefundTransactionHandler : IRequestHandler<RefundTransactionCommand, OperationResult>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public RefundTransactionHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<OperationResult> Handle(RefundTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
            if (transaction == null)
            {
                return OperationResult.NotFound("تراکنش یافت نشد");
            }

            // بررسی امکان بازگشت وجه
            if (transaction.Status != TransactionStatus.Completed)
            {
                return OperationResult.Error("فقط تراکنش‌های تکمیل شده قابل بازگشت وجه هستند");
            }

            if (transaction.Status == TransactionStatus.Refunded)
            {
                return OperationResult.Error("این تراکنش قبلاً بازگشت وجه شده است");
            }

            // تعیین مبلغ بازگشتی
            var refundAmount = request.RefundAmount ?? transaction.Amount;
            
            if (refundAmount > transaction.Amount)
            {
                return OperationResult.Error("مبلغ بازگشتی نمی‌تواند بیشتر از مبلغ اصلی تراکنش باشد");
            }

            // بروزرسانی تراکنش
            transaction.Status = TransactionStatus.Refunded;
            transaction.RefundReason = request.Reason;
            transaction.RefundAmount = refundAmount;
            transaction.RefundedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            _transactionRepository.Update(transaction);
            await _transactionRepository.SaveChangesAsync(cancellationToken);

            // TODO: در اینجا می‌توان لاگ بازگشت وجه را ثبت کرد
            // TODO: ارسال اعلان به کاربر

            return OperationResult.Success("بازگشت وجه با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"خطا در بازگشت وجه: {ex.Message}");
        }
    }
}