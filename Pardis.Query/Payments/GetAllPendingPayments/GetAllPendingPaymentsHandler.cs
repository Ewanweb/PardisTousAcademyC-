using AutoMapper;
using MediatR;
using Pardis.Application.Shopping.Contracts;
using Pardis.Domain.Dto.Payments;
using Pardis.Domain.Shopping;
using System.Text.Json;

namespace Pardis.Query.Payments.GetAllPendingPayments;

/// <summary>
/// Handler برای دریافت تمام پرداخت‌های در انتظار تایید - ساده شده برای PaymentAttempt فقط
/// </summary>
public class GetAllPendingPaymentsHandler : IRequestHandler<GetAllPendingPaymentsQuery, GetAllPendingPaymentsResult>
{
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IMapper _mapper;

    public GetAllPendingPaymentsHandler(
        IPaymentAttemptRepository paymentAttemptRepository,
        IMapper mapper)
    {
        _paymentAttemptRepository = paymentAttemptRepository;
        _mapper = mapper;
    }

    public async Task<GetAllPendingPaymentsResult> Handle(GetAllPendingPaymentsQuery request, CancellationToken cancellationToken)
    {
        // دریافت تلاش‌های پرداخت در انتظار تایید ادمین
        var paymentAttempts = await _paymentAttemptRepository.GetPendingAdminApprovalAsync(cancellationToken);

        var result = new GetAllPendingPaymentsResult
        {
            // تبدیل PaymentAttempts به ManualPaymentRequestDto برای سازگاری با فرانت‌اند
            ManualPayments = _mapper.Map<List<ManualPaymentRequestDto>>(paymentAttempts),
            PaymentAttempts = paymentAttempts.Select(pa => new PendingPaymentAttemptDto
            {
                Id = pa.Id,
                TrackingCode = pa.TrackingCode ?? string.Empty,
                StudentName = pa.User?.FullName ?? pa.User?.UserName ?? pa.User?.Email ?? "نامشخص",
                StudentId = pa.UserId,
                Amount = pa.Amount,
                ReceiptUrl = pa.ReceiptImageUrl ?? string.Empty,
                ReceiptUploadedAt = pa.ReceiptUploadedAt ?? DateTime.MinValue,
                CreatedAt = pa.CreatedAt,
                OrderNumber = pa.Order?.OrderNumber ?? string.Empty,
                CourseNames = ExtractCourseNames(pa.Order?.CartSnapshot ?? string.Empty)
            }).ToList()
        };

        return result;
    }

    private List<string> ExtractCourseNames(string cartSnapshot)
    {
        try
        {
            if (string.IsNullOrEmpty(cartSnapshot))
                return new List<string>();

            var items = JsonSerializer.Deserialize<List<JsonElement>>(cartSnapshot);
            return items?.Select(item => 
                item.TryGetProperty("Title", out JsonElement title) ? 
                title.GetString() ?? "نامشخص" : "نامشخص"
            ).ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}