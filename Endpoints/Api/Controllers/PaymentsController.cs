using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Pardis.Domain.Payments;
using Pardis.Application.Payments.ManualPayment.CreateManualPayment;
using Pardis.Application.Payments.ManualPayment.UploadReceipt;
using Pardis.Application.Payments.ManualPayment.ReviewManualPayment;
using Pardis.Query.Payments;
using Pardis.Query.Settings;
using MediatR;

namespace Api.Controllers;

/// <summary>
/// کنترلر پردازش پرداخت‌ها برای کاربران
/// </summary>
[Route("api/payments")]
[Authorize]
public class PaymentsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر پرداخت‌ها
    /// </summary>
    public PaymentsController(ILogger<PaymentsController> logger, IMediator mediator) 
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت اطلاعات کارت مقصد برای پرداخت دستی
    /// </summary>
    [HttpGet("manual/info")]
    public async Task<IActionResult> GetManualPaymentInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetManualPaymentInfoQuery();
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "اطلاعات کارت مقصد");
        }, "خطا در دریافت اطلاعات کارت");
    }

    /// <summary>
    /// ایجاد درخواست پرداخت دستی برای دوره
    /// </summary>
    [HttpPost("courses/{courseId}/purchase/manual")]
    public async Task<IActionResult> CreateManualPaymentRequest(Guid courseId, [FromBody] CreateManualPaymentRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new CreateManualPaymentCommand
            {
                CourseId = courseId,
                StudentId = userId,
                Amount = request.Amount
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در ایجاد درخواست پرداخت");
    }

    /// <summary>
    /// آپلود رسید پرداخت دستی
    /// </summary>
    [HttpPost("manual/{paymentId}/receipt")]
    public async Task<IActionResult> UploadReceipt(Guid paymentId, [FromForm] UploadReceiptDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new UploadReceiptCommand
            {
                PaymentRequestId = paymentId,
                StudentId = userId,
                ReceiptFile = request.ReceiptFile
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در آپلود رسید");
    }

    /// <summary>
    /// دریافت درخواست‌های پرداخت دستی کاربر جاری
    /// </summary>
    [HttpGet("manual/my-requests")]
    public async Task<IActionResult> GetMyManualPaymentRequests()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetManualPaymentRequestsQuery { StudentId = userId };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "درخواست‌های پرداخت دستی شما");
        }, "خطا در دریافت درخواست‌های پرداخت");
    }

    /// <summary>
    /// دریافت اقساط کاربر جاری
    /// </summary>
    [HttpGet("my-enrollments")]
    public async Task<IActionResult> GetMyEnrollments()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetStudentEnrollmentsQuery { StudentId = userId };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "اقساط و پرداخت‌های شما");
        }, "خطا در دریافت اقساط");
    }

    /// <summary>
    /// دریافت لیست درخواست‌های پرداخت دستی برای ادمین
    /// </summary>
    [HttpGet("admin/manual")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetManualPaymentRequests([FromQuery] string? status = null)
    {
        return await ExecuteAsync(async () =>
        {
            ManualPaymentStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ManualPaymentStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            var query = new GetManualPaymentRequestsQuery { Status = statusEnum };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "لیست درخواست‌های پرداخت دستی");
        }, "خطا در دریافت درخواست‌های پرداخت");
    }

    /// <summary>
    /// تایید یا رد درخواست پرداخت دستی توسط ادمین
    /// </summary>
    [HttpPost("admin/manual/{paymentId}/review")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReviewManualPayment(Guid paymentId, [FromBody] ReviewManualPaymentDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new ReviewManualPaymentCommand
            {
                PaymentRequestId = paymentId,
                AdminUserId = userId,
                IsApproved = request.IsApproved,
                RejectReason = request.RejectReason
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در بررسی درخواست پرداخت");
    }
}

/// <summary>
/// درخواست ایجاد پرداخت دستی
/// </summary>
public class CreateManualPaymentRequestDto
{
    /// <summary>
    /// مبلغ پرداخت
    /// </summary>
    public long Amount { get; set; }
}

/// <summary>
/// درخواست آپلود رسید
/// </summary>
public class UploadReceiptDto
{
    /// <summary>
    /// فایل رسید
    /// </summary>
    public IFormFile ReceiptFile { get; set; } = null!;
}

/// <summary>
/// درخواست بررسی پرداخت دستی
/// </summary>
public class ReviewManualPaymentDto
{
    /// <summary>
    /// آیا تایید شده است؟
    /// </summary>
    public bool IsApproved { get; set; }
    
    /// <summary>
    /// دلیل رد (در صورت رد)
    /// </summary>
    public string? RejectReason { get; set; }
}