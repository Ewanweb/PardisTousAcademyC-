using Api.Attributes;
using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application._Shared.Pagination;
using Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;
using Pardis.Query.Payments.GetAllPayments;
using Pardis.Query.Shopping.GetPendingPayments;
using Pardis.Query.Shopping.GetPaymentAttempt;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت پرداخت‌ها در پنل ادمین
/// </summary>
[Area("Admin")]
[Route("api/admin/payments")]
[Authorize(Policy = Policies.PaymentManagement.Access)]
public class PaymentManagementController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IIdempotencyService _idempotencyService;

    /// <summary>
    /// سازنده کنترلر مدیریت پرداخت‌ها
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    /// <param name="idempotencyService">سرویس idempotency</param>
    public PaymentManagementController(
        IMediator mediator,
        ILogger<PaymentManagementController> logger,
        IIdempotencyService idempotencyService
    ) : base(mediator, logger)
    {
        _mediator = mediator;
        _idempotencyService = idempotencyService;
    }

    /// <summary>
    /// دریافت لیست پرداخت‌های در انتظار بررسی
    /// </summary>
    [HttpGet("pending-receipts")]
    public async Task<IActionResult> GetPendingReceipts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        return await ExecuteAsync(async () =>
        {
            var pagination = GetPaginationRequest(page, pageSize);
            var query = new GetPendingPaymentsQuery
            {
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };

            var result = await _mediator.Send(query);

            return SuccessResponse(
                result,
                "لیست پرداخت‌های در انتظار بررسی با موفقیت دریافت شد"
            );
        },
        "خطا در دریافت لیست پرداخت‌های در انتظار بررسی");
    }

    /// <summary>
    /// تأیید پرداخت توسط مدیر
    /// </summary>
    [HttpPost("{paymentAttemptId}/approve")]
    [Authorize(Policy = Policies.PaymentManagement.AdminActions)]
    [RateLimit(MaxRequests = 10, WindowMinutes = 1)] // Rate limiting
    public async Task<IActionResult> ApprovePayment(
        Guid paymentAttemptId, 
        [FromBody] ApprovePaymentRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            if (string.IsNullOrEmpty(idempotencyKey))
                return BadRequestResponse("Idempotency key is required");

            // Check if payment attempt exists
            var paymentAttempt = await _mediator.Send(new GetPaymentAttemptQuery(paymentAttemptId));
            if (paymentAttempt == null)
                return BadRequestResponse("Payment attempt not found");

            // Create command with all required fields
            var command = new AdminReviewPaymentCommand
            {
                PaymentAttemptId = paymentAttemptId,
                AdminUserId = userId,
                IsApproved = request.IsApproved,
                AdminNote = request.AdminNote,
                RejectReason = request.RejectionReason,
                ApprovedAmount = request.ApprovedAmount,
                IdempotencyKey = idempotencyKey,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent()
            };

            // Execute with idempotency
            var result = await _idempotencyService.ExecuteWithIdempotencyAsync(
                idempotencyKey,
                userId,
                "approve_payment",
                command,
                async (cancellationToken) => await _mediator.Send(command, cancellationToken));

            if (!result.IsSuccess)
                return ErrorResponse(result.ErrorMessage ?? "خطا در تایید پرداخت");

            if (result.IsReplayed)
                return SuccessResponse(result.Data, "عملیات قبلاً انجام شده است");

            return SuccessResponse(
                result.Data,
                request.IsApproved ? "پرداخت با موفقیت تأیید شد" : "پرداخت رد شد"
            );
        },
        "خطا در تأیید پرداخت");
    }


    [HttpPost("{paymentAttemptId}/reject")]
    [Authorize(Policy = Policies.PaymentManagement.AdminActions)]
    [RateLimit(MaxRequests = 10, WindowMinutes = 1)]
    public async Task<IActionResult> RejectPayment(
        Guid paymentAttemptId, 
        [FromBody] RejectPaymentRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            if (string.IsNullOrEmpty(idempotencyKey))
                return BadRequestResponse("Idempotency-Key header is required");

            if (!await HasPaymentApprovalPermission(userId, paymentAttemptId))
                return ForbiddenResponse("Insufficient permissions for this payment");

            var command = new AdminReviewPaymentCommand
            {
                PaymentAttemptId = paymentAttemptId,
                AdminUserId = userId,
                IsApproved = false,
                RejectReason = request.Reason,
                IdempotencyKey = idempotencyKey,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _idempotencyService.ExecuteWithIdempotencyAsync(
                idempotencyKey,
                userId,
                "PaymentRejection",
                command,
                async (ct) => await _mediator.Send(command, ct));

            if (!result.IsSuccess)
                return ErrorResponse(result.ErrorMessage ?? "خطا در رد پرداخت");

            if (result.IsReplayed)
                return SuccessResponse(result.Data, "عملیات قبلاً انجام شده است");

            return SuccessResponse(result.Data, "پرداخت رد شد");
        },
        "خطا در رد پرداخت");
    }

    // CRITICAL: Additional authorization check
    private async Task<bool> HasPaymentApprovalPermission(string userId, Guid paymentAttemptId)
    {
        try
        {
            // Check if payment exists (don't filter by userId for admin check)
            var payment = await _mediator.Send(new GetPaymentAttemptQuery 
            { 
                PaymentAttemptId = paymentAttemptId,
                UserId = null // Admin can see all payments
            });
            
            if (payment == null)
                return false;

            // Additional business rules can be added here
            // e.g., payment amount limits, user approval limits, etc.
            
            return true; // User has Manager role (checked by policy)
        }
        catch
        {
            return false;
        }
    }

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string GetUserAgent()
    {
        return HttpContext.Request.Headers["User-Agent"].ToString();
    }

    private IActionResult ForbiddenResponse(string message)
    {
        return StatusCode(403, new { success = false, message });
    }


    /// <summary>
    /// دریافت لیست تمامی پرداخت‌ها با امکان فیلتر
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? status = null
    )
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetAllPaymentsQuery
            {
                Pagination = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize
                },
                Search = search,
                Status = status
            };

            var result = await _mediator.Send(query);

            return HandleOperationResult(
                result,
                "لیست پرداخت‌ها با موفقیت دریافت شد"
            );
        },
        "خطا در دریافت لیست پرداخت‌ها");
    }
}

/// <summary>
/// درخواست رد کردن پرداخت
/// </summary>
public class RejectPaymentRequest
{
    /// <summary>
    /// دلیل رد شدن پرداخت
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
