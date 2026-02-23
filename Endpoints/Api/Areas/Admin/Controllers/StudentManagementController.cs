using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Payments;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;
using Pardis.Query.Students;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت دانشجویان
/// </summary>
[Route("api/admin/students")]
[ApiController]
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.FinancialManager)]
[Produces("application/json")]
[Tags("Student Management")]
public class StudentManagementController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر مدیریت دانشجویان
    /// </summary>
    public StudentManagementController(IMediator mediator, ILogger<StudentManagementController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت پروفایل مالی دانشجو
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <returns>پروفایل مالی دانشجو شامل تمام ثبت‌نام‌ها و پرداخت‌ها</returns>
    /// <response code="200">پروفایل مالی با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی</response>
    /// <response code="404">دانشجو یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{studentId}/financial-profile")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentFinancialProfile(string studentId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            var query = new GetStudentFinancialProfileQuery { StudentId = studentId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFoundResponse("دانشجو یافت نشد");

            return SuccessResponse(result, "پروفایل مالی دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت پروفایل مالی دانشجو");
    }

    /// <summary>
    /// ثبت پرداخت دستی برای یک ثبت‌نام
    /// </summary>
    /// <param name="enrollmentId">شناسه ثبت‌نام</param>
    /// <param name="request">اطلاعات پرداخت</param>
    /// <returns>نتیجه ثبت پرداخت</returns>
    /// <response code="200">پرداخت با موفقیت ثبت شد</response>
    /// <response code="400">درخواست نامعتبر یا مبلغ بیشتر از باقی‌مانده</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی</response>
    /// <response code="404">ثبت‌نام یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("enrollments/{enrollmentId}/payments")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
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
                return ValidationErrorResponse("مبلغ پرداخت نامعتبر است", new { amount = "مبلغ باید بیشتر از صفر باشد" });

            if (string.IsNullOrWhiteSpace(request.PaymentReference))
                return ValidationErrorResponse("شماره مرجع پرداخت الزامی است", new { paymentReference = "شماره مرجع نمی‌تواند خالی باشد" });

            var command = new RecordPaymentCommand
            {
                EnrollmentId = enrollmentId,
                Amount = request.Amount,
                PaymentReference = request.PaymentReference,
                Method = request.Method,
                Notes = request.Notes
            };

            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.Success)
                return SuccessResponse(result.Data, "پرداخت با موفقیت ثبت شد");

            if (result.Status == OperationResultStatus.NotFound)
                return NotFoundResponse(result.Message ?? "ثبت‌نام یافت نشد");

            if (result.Status == OperationResultStatus.Error)
                return ErrorResponse(result.Message ?? "خطا در ثبت پرداخت", 400, "PAYMENT_RECORD_FAILED");

            return HandleOperationResult(result);
        }, "خطا در ثبت پرداخت");
    }
}

/// <summary>
/// مدل درخواست ثبت پرداخت
/// </summary>
public class RecordPaymentRequest
{
    /// <summary>
    /// مبلغ پرداخت (به ریال)
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// شماره مرجع پرداخت (شماره فیش، شماره تراکنش و...)
    /// </summary>
    public string PaymentReference { get; set; } = string.Empty;

    /// <summary>
    /// روش پرداخت (0: آنلاین, 1: نقدی, 2: انتقال بانکی, 3: چک)
    /// </summary>
    public EnrollmentPaymentMethod Method { get; set; } = EnrollmentPaymentMethod.Cash;

    /// <summary>
    /// یادداشت (اختیاری)
    /// </summary>
    public string? Notes { get; set; }
}
