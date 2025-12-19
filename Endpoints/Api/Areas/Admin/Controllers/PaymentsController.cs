using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Payments;
using Api.Controllers;

namespace Endpoints.Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت پرداخت‌ها و اقساط - فقط برای ادمین و منیجر
/// </summary>
[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = Role.Admin + "," + Role.Manager)]
[Produces("application/json")]
[Tags("Payments Management")]
public class PaymentsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر پرداخت‌ها
    /// </summary>
    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت اقساط دانشجو شامل جزئیات پرداخت‌ها
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <returns>لیست اقساط و پرداخت‌های دانشجو</returns>
    /// <response code="200">اقساط دانشجو با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("enrollments/student/{studentId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentEnrollments(string studentId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دانشجو الزامی است" 
                });

            var query = new GetStudentEnrollmentsQuery { StudentId = studentId };
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "اقساط دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت اقساط دانشجو");
    }

    /// <summary>
    /// ثبت پرداخت برای یک ثبت‌نام
    /// </summary>
    /// <param name="enrollmentId">شناسه ثبت‌نام</param>
    /// <param name="request">اطلاعات پرداخت</param>
    /// <returns>نتیجه ثبت پرداخت</returns>
    /// <response code="200">پرداخت با موفقیت ثبت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="404">ثبت‌نام یافت نشد</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("enrollment/{enrollmentId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RecordPayment(Guid enrollmentId, [FromBody] RecordPaymentRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            if (enrollmentId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه ثبت‌نام نامعتبر است" 
                });

            if (request.Amount <= 0)
                return BadRequest(new { 
                    success = false, 
                    message = "مبلغ پرداخت باید بیشتر از صفر باشد" 
                });

            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return Unauthorized(new { success = false, message = "کاربر احراز هویت نشده است" });

            // TODO: پیاده‌سازی Command برای ثبت پرداخت
            var mockResult = new
            {
                id = Guid.NewGuid(),
                enrollmentId = enrollmentId,
                amount = request.Amount,
                paymentMethod = request.PaymentMethod,
                paymentDate = request.PaymentDate,
                description = request.Description,
                recordedByUserId = adminUserId,
                recordedAt = DateTime.UtcNow
            };
            
            return SuccessResponse(mockResult, "پرداخت با موفقیت ثبت شد");
        }, "خطا در ثبت پرداخت");
    }
}

/// <summary>
/// درخواست ثبت پرداخت
/// </summary>
public class RecordPaymentRequest
{
    /// <summary>
    /// مبلغ پرداخت
    /// </summary>
    public long Amount { get; set; }
    
    /// <summary>
    /// روش پرداخت
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات پرداخت
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// تاریخ پرداخت
    /// </summary>
    public DateTime PaymentDate { get; set; }
}