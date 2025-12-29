using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Application.Payments.ManualPayment;

/// <summary>
/// Command آپلود رسید پرداخت دستی
/// </summary>
public class UploadReceiptCommand : IRequest<OperationResult<ManualPaymentRequestDto>>
{
    public Guid PaymentRequestId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public IFormFile ReceiptFile { get; set; } = null!;
}