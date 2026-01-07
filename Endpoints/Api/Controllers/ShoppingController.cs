using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Pardis.Application.Shopping.Cart.AddCourseToCart;
using Pardis.Application.Shopping.Cart.RemoveCourseFromCart;
using Pardis.Application.Shopping.Cart.ClearCart;
using Pardis.Application.Shopping.Checkout.CreateCheckout;
using Pardis.Query.Shopping.GetMyCart;
using Pardis.Query.Shopping.GetMyOrders;
using Pardis.Query.Shopping.GetPaymentAttempt;
using Pardis.Application.Shopping.PaymentAttempts.UploadReceipt;
using Pardis.Domain.Shopping;

namespace Api.Controllers;

/// <summary>
/// کنترلر سبد خرید و سفارش‌ها
/// </summary>
[Route("api/me")]
[Authorize(Policy = Policies.Shopping.Access)]
public class ShoppingController : BaseController
{
    private readonly IMediator _mediator;

    public ShoppingController(ILogger<ShoppingController> logger, IMediator mediator) 
        : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت سبد خرید کاربر جاری
    /// </summary>
    [HttpGet("cart")]
    public async Task<IActionResult> GetMyCart()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetMyCartQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return SuccessResponse(result, "سبد خرید شما");
        }, "خطا در دریافت سبد خرید");
    }

    /// <summary>
    /// اضافه کردن دوره به سبد خرید
    /// </summary>
    [HttpPost("cart/items")]
    public async Task<IActionResult> AddCourseToCart([FromBody] AddCourseToCartRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new AddCourseToCartCommand
            {
                UserId = userId,
                CourseId = request.CourseId
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در اضافه کردن دوره به سبد خرید");
    }

    /// <summary>
    /// حذف دوره از سبد خرید
    /// </summary>
    [HttpDelete("cart/items/{courseId}")]
    public async Task<IActionResult> RemoveCourseFromCart(Guid courseId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new RemoveCourseFromCartCommand
            {
                UserId = userId,
                CourseId = courseId
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در حذف دوره از سبد خرید");
    }

    /// <summary>
    /// پاک کردن کل سبد خرید
    /// </summary>
    [HttpDelete("cart")]
    public async Task<IActionResult> ClearCart()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new ClearCartCommand { UserId = userId };
            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در پاک کردن سبد خرید");
    }

    /// <summary>
    /// ایجاد سفارش (checkout)
    /// </summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var idempotencyKey = Request.Headers["X-Idempotency-Key"].ToString();

            var command = new CreateCheckoutCommand
            {
                UserId = userId,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes,
                IdempotencyKey = string.IsNullOrEmpty(idempotencyKey) ? null : idempotencyKey
            };

            var result = await _mediator.Send(command);
            
            if (result.Status != Pardis.Application._Shared.OperationResultStatus.Success)
                return ErrorResponse(result.Message);

            return SuccessResponse(result.Data, result.Message);
        }, "خطا در ایجاد سفارش");
    }

    /// <summary>
    /// دریافت سفارش‌های کاربر جاری
    /// </summary>
    [HttpGet("orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

            return SuccessResponse(result, "سفارش‌های شما");
        }, "خطا در دریافت سفارش‌ها");
    }

    /// <summary>
    /// دریافت جزئیات یک تلاش پرداخت
    /// </summary>
    [HttpGet("payments/{paymentId}")]
    public async Task<IActionResult> GetPaymentAttempt(Guid paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetPaymentAttemptQuery
            {
                PaymentAttemptId = paymentId,
                UserId = userId
            };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFoundResponse("تلاش پرداخت یافت نشد");

            return SuccessResponse(result, "جزئیات پرداخت");
        }, "خطا در دریافت جزئیات پرداخت");
    }

    /// <summary>
    /// آپلود رسید پرداخت دستی
    /// </summary>
    [HttpPost("payments/{paymentId}/receipt")]
    public async Task<IActionResult> UploadReceipt(Guid paymentId, [FromForm] UploadReceiptRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var command = new UploadReceiptCommand
            {
                PaymentAttemptId = paymentId,
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
    [HttpGet("payments/{paymentId}/receipt")]
    public async Task<IActionResult> GetReceipt(Guid paymentId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse();

            var query = new GetPaymentAttemptQuery
            {
                PaymentAttemptId = paymentId,
                UserId = userId
            };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFoundResponse("تلاش پرداخت یافت نشد");

            if (string.IsNullOrEmpty(result.ReceiptUrl))
                return NotFoundResponse("رسید برای این پرداخت یافت نشد");

            // برگرداندن URL رسید
            return SuccessResponse(new { receiptUrl = result.ReceiptUrl }, "رسید پرداخت");
        }, "خطا در دریافت رسید");
    }
}



/// <summary>
/// درخواست اضافه کردن دوره به سبد خرید
/// </summary>
public class AddCourseToCartRequest
{
    /// <summary>
    /// شناسه دوره
    /// </summary>
    public Guid CourseId { get; set; }
}

/// <summary>
/// درخواست ایجاد سفارش
/// </summary>
public class CreateCheckoutRequest
{
    /// <summary>
    /// روش پرداخت
    /// </summary>
    [System.ComponentModel.DataAnnotations.Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    /// <summary>
    /// یادداشت‌ها
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// کلید جلوگیری از تکرار (اختیاری در بدنه، ترجیحاً در هدر X-Idempotency-Key)
    /// </summary>
    public string? IdempotencyKey { get; set; }
}