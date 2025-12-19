using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Handler برای بروزرسانی وضعیت تراکنش
/// </summary>
public class UpdateTransactionStatusHandler : IRequestHandler<UpdateTransactionStatusCommand, OperationResult>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public UpdateTransactionStatusHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<OperationResult> Handle(UpdateTransactionStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
            if (transaction == null)
            {
                return OperationResult.NotFound("تراکنش یافت نشد");
            }

            transaction.Status = request.Status;
            
            if (!string.IsNullOrEmpty(request.GatewayTransactionId))
                transaction.GatewayTransactionId = request.GatewayTransactionId;

            if (!string.IsNullOrEmpty(request.Description))
                transaction.Description = request.Description;

            transaction.UpdatedAt = DateTime.UtcNow;

            _transactionRepository.Update(transaction);
            await _transactionRepository.SaveChangesAsync(cancellationToken);

            return OperationResult.Success("وضعیت تراکنش با موفقیت بروزرسانی شد");
        }
        catch (Exception ex)
        {
            return OperationResult.Error($"خطا در بروزرسانی تراکنش: {ex.Message}");
        }
    }
}