using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Pardis.Application.Shopping.Cart.AddCourseToCart;
using Pardis.Application.Shopping.Cart.RemoveCourseFromCart;
using Pardis.Application.Shopping.Cart.ClearCart;
using Pardis.Application.Shopping.Checkout.CreateCheckout;
using Pardis.Query.Shopping.GetMyCart;
using Pardis.Query.Shopping.GetMyOrders;

namespace Api.Controllers;

/// <summary>
/// کنترلر سبد خرید و سفارش‌ها
/// </summary>
[Route("api/me")]
[Authorize]
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

            var command = new CreateCheckoutCommand
            {
                UserId = userId,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes
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
    public Pardis.Domain.Shopping.PaymentMethod PaymentMethod { get; set; }
    
    /// <summary>
    /// یادداشت‌ها
    /// </summary>
    public string? Notes { get; set; }
}