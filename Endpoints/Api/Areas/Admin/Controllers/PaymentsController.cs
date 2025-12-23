using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Payments;
using Pardis.Application.Payments;
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
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            var query = new GetStudentEnrollmentsQuery { StudentId = studentId.Trim() };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return SuccessResponse(new List<object>(), "هیچ ثبت‌نامی برای این دانشجو یافت نشد");

            // فرض می‌کنیم result از نوع IEnumerable است
            var enrollments = result as IEnumerable<object>;
            if (enrollments != null && enrollments.Any())
            {
                return SuccessResponse(result, $"اقساط {enrollments.Count()} ثبت‌نام دانشجو دریافت شد");
            }

            return SuccessResponse(new List<object>(), "هیچ ثبت‌نامی برای این دانشجو یافت نشد");
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
                return ValidationErrorResponse("شناسه ثبت‌نام نامعتبر است", new { enrollmentId = "شناسه ثبت‌نام نمی‌تواند خالی باشد" });

            if (request == null)
                return ValidationErrorResponse("اطلاعات پرداخت الزامی است");

            if (request.Amount <= 0)
                return ValidationErrorResponse("مبلغ پرداخت نامعتبر است", new { amount = "مبلغ پرداخت باید بیشتر از صفر باشد" });

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                return ValidationErrorResponse("روش پرداخت الزامی است", new { paymentMethod = "روش پرداخت نمی‌تواند خالی باشد" });

            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            var command = new RecordPaymentCommand
            {
                EnrollmentId = enrollmentId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod.Trim(),
                Description = request.Description?.Trim(),
                PaymentDate = request.PaymentDate,
                RecordedByUserId = adminUserId
            };

            var result = await _mediator.Send(command);
            
            if (result == null)
                return ErrorResponse("خطا در ثبت پرداخت", 500, "RECORD_PAYMENT_FAILED");

            return SuccessResponse(result, "پرداخت با موفقیت ثبت شد");
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