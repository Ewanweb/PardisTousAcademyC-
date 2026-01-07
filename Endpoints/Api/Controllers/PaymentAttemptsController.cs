using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Pardis.Application.Shopping.PaymentAttempts.UploadReceipt;
using Pardis.Query.Shopping.GetMyOrders;

namespace Api.Controllers;

/// <summary>
/// کنترلر تلاش‌های پرداخت - ساده شده برای پرداخت دستی فقط
/// </summary>
[Route("api/me/payment-attempts")]
[Authorize]
public class PaymentAttemptsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر تلاش‌های پرداخت
    /// </summary>
    public PaymentAttemptsController(ILogger<PaymentAttemptsController> logger, IMediator mediator) 
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// آپلود رسید پرداخت دستی
    /// </summary>
    [HttpPost("{paymentAttemptId}/receipt")]
    public async Task<IActionResult> UploadReceipt(Guid paymentAttemptId, [FromForm] UploadReceiptRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new UploadReceiptCommand
            {
                PaymentAttemptId = paymentAttemptId,
                UserId = userId,
                ReceiptFile = request.ReceiptFile
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در آپلود رسید");
    }

    /// <summary>
    /// دریافت رسید پرداخت
    /// </summary>
    [HttpGet("{paymentAttemptId}/receipt")]
    public async Task<IActionResult> GetReceipt(Guid paymentAttemptId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new Pardis.Query.Shopping.GetPaymentReceipt.GetPaymentReceiptQuery
            {
                PaymentAttemptId = paymentAttemptId,
                UserId = userId
            };

            var result = await _mediator.Send(query);
            if (result == null)
                return NotFoundResponse("رسید پرداخت یافت نشد");

            return SuccessResponse(result, "رسید پرداخت");
        }, "خطا در دریافت رسید");
    }

    /// <summary>
    /// دریافت تاریخچه پرداخت‌های کاربر (همان GetMyOrders)
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> GetMyPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetMyOrdersQuery 
            { 
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "تاریخچه پرداخت‌های شما");
        }, "خطا در دریافت تاریخچه پرداخت‌ها");
    }
}

/// <summary>
/// درخواست آپلود رسید
/// </summary>
public class UploadReceiptRequest
{
    /// <summary>
    /// فایل رسید
    /// </summary>
    public IFormFile ReceiptFile { get; set; } = null!;
}