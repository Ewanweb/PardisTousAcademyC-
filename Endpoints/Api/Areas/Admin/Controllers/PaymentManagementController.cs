using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;
using Pardis.Query.Payments.GetAllPayments;
using Pardis.Query.Shopping.GetPendingPayments;

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

    /// <summary>
    /// سازنده کنترلر مدیریت پرداخت‌ها
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    public PaymentManagementController(
        IMediator mediator,
        ILogger<PaymentManagementController> logger
    ) : base(mediator, logger)
    {
        _mediator = mediator;
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
    public async Task<IActionResult> ApprovePayment(Guid paymentAttemptId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new AdminReviewPaymentCommand
            {
                PaymentAttemptId = paymentAttemptId,
                AdminUserId = userId,
                IsApproved = true
            };

            var result = await _mediator.Send(command);

            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        },
        "خطا در تأیید پرداخت");
    }

    /// <summary>
    /// رد کردن پرداخت توسط مدیر به همراه دلیل
    /// </summary>
    [HttpPost("{paymentAttemptId}/reject")]
    public async Task<IActionResult> RejectPayment(
        Guid paymentAttemptId,
        [FromBody] RejectPaymentRequest request
    )
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new AdminReviewPaymentCommand
            {
                PaymentAttemptId = paymentAttemptId,
                AdminUserId = userId,
                IsApproved = false,
                RejectReason = request.Reason
            };

            var result = await _mediator.Send(command);

            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        },
        "خطا در رد کردن پرداخت");
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
            var pagination = GetPaginationRequest(page, pageSize);
            var query = new GetAllPaymentsQuery
            {
                Pagination = pagination,
                Search = search,
                Status = status
            };

            var result = await _mediator.Send(query);

            return SuccessResponse(
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
