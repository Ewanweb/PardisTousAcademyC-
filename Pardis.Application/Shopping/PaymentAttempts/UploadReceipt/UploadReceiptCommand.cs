using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.PaymentAttempts.UploadReceipt;

/// <summary>
/// دستور آپلود رسید پرداخت
/// </summary>
public class UploadReceiptCommand : IRequest<OperationResult<UploadReceiptResult>>
{
    public Guid PaymentAttemptId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IFormFile ReceiptFile { get; set; } = null!;
}

/// <summary>
/// نتیجه آپلود رسید
/// </summary>
public class UploadReceiptResult
{
    public Guid PaymentAttemptId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public string ReceiptUrl { get; set; } = string.Empty;
    public string ReceiptFileName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public int Status { get; set; }
    public string? RejectReason { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}