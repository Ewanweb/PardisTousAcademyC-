using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Query.Payments.GetAllPendingPayments;
using Pardis.Query.Settings;

namespace Api.Controllers;

/// <summary>
/// کنترلر پردازش پرداخت‌ها برای کاربران - فقط پرداخت دستی
/// </summary>
[Route("api/payments")]
[Authorize(Policy = Policies.Payments.AdminActions)]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetManualPaymentInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetManualPaymentInfoQuery();
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "اطلاعات کارت مقصد");
        }, "خطا در دریافت اطلاعات کارت");
    }


}