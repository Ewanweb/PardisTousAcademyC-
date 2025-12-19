using AutoMapper;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Dto.Accounting;

namespace Pardis.Application.Accounting;

/// <summary>
/// Handler برای ایجاد تراکنش جدید
/// </summary>
public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, OperationResult<TransactionDto>>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IMapper _mapper;

    public CreateTransactionHandler(IRepository<Transaction> transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<TransactionDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // تولید شناسه یکتا تراکنش
            var transactionId = GenerateTransactionId();

            var transaction = new Transaction
            {
                TransactionId = transactionId,
                UserId = request.UserId,
                CourseId = request.CourseId,
                Amount = request.Amount,
                Status = TransactionStatus.Pending,
                Method = request.Method,
                Gateway = request.Gateway,
                GatewayTransactionId = request.GatewayTransactionId,
                Description = request.Description
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync(cancellationToken);

            var transactionDto = _mapper.Map<TransactionDto>(transaction);

            return OperationResult<TransactionDto>.Success(transactionDto);
        }
        catch (Exception ex)
        {
            return OperationResult<TransactionDto>.Error($"خطا در ایجاد تراکنش: {ex.Message}");
        }
    }

    private static string GenerateTransactionId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        return $"TXN-{timestamp}-{random}";
    }
}