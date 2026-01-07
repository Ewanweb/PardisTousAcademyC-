using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;
using Pardis.Query.Payments.GetAllPayments;
using Pardis.Query.Payments.GetAllPendingPayments;
using Pardis.Query.Shopping.GetPendingPayments;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت پرداخت‌ها برای ادمین
/// </summary>
[Area("Admin")]
[Route("api/admin/payments")]
[Authorize(Policy = Policies.PaymentManagement.Access)]
public class PaymentManagementController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor for payment management controller
    /// </summary>
    /// <param name="mediator">MediatR instance</param>
    /// <param name="logger">Logger instance</param>
    public PaymentManagementController(IMediator mediator, ILogger<PaymentManagementController> logger) 
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت پرداخت‌های در انتظار تایید ادمین
    /// </summary>
    [HttpGet("pending-receipts")]
    public async Task<IActionResult> GetPendingReceipts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetPendingPaymentsQuery 
            { 
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "پرداخت‌های در انتظار تایید");
        }, "خطا در دریافت پرداخت‌های در انتظار تایید");
    }

    /// <summary>
    /// تایید پرداخت توسط ادمین
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
        }, "خطا در تایید پرداخت");
    }

    /// <summary>
    /// رد پرداخت توسط ادمین
    /// </summary>
    [HttpPost("{paymentAttemptId}/reject")]
    public async Task<IActionResult> RejectPayment(Guid paymentAttemptId, [FromBody] RejectPaymentRequest request)
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
        }, "خطا در رد پرداخت");
    }
    /// <summary>
    /// پرداخت ها
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GeGetAllPayments()
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetAllPaymentsQuery();
            var result = await _mediator.Send(query);
            return SuccessResponse(result, "روش‌های پرداخت");
        }, "خطا در دریافت روش‌های پرداخت");
    }
}

/// <summary>
/// درخواست رد پرداخت
/// </summary>
public class RejectPaymentRequest
{
    /// <summary>
    /// دلیل رد
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}