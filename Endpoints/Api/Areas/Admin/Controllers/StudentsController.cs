using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Students;

namespace Endpoints.Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت دانشجویان - فقط برای ادمین و منیجر
/// </summary>
[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Policy = Policies.StudentManagement.Access)]
[Produces("application/json")]
[Tags("Students Management")]
public class StudentsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر مدیریت دانشجویان
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    public StudentsController(IMediator mediator, ILogger<StudentsController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت پروفایل کامل دانشجو شامل اطلاعات شخصی، دوره‌ها و آمار
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <returns>پروفایل کامل دانشجو</returns>
    /// <response code="200">پروفایل دانشجو با موفقیت دریافت شد</response>
    /// <response code="404">دانشجو یافت نشد</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{studentId}/profile")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentProfile(string studentId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            var query = new GetStudentProfileQuery { StudentId = studentId.Trim() };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFoundResponse("دانشجو یافت نشد");
            
            return SuccessResponse(result, "پروفایل دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت پروفایل دانشجو");
    }

    /// <summary>
    /// دریافت خلاصه مالی دانشجو شامل مبالغ پرداختی، باقی‌مانده و اقساط معوق
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <returns>خلاصه مالی دانشجو</returns>
    /// <response code="200">خلاصه مالی دانشجو با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="404">دانشجو یافت نشد</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{studentId}/financial-summary")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentFinancialSummary(string studentId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            var query = new GetStudentFinancialSummaryQuery { StudentId = studentId.Trim() };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFoundResponse("دانشجو یافت نشد یا اطلاعات مالی در دسترس نیست");
            
            return SuccessResponse(result, "خلاصه مالی دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت خلاصه مالی دانشجو");
    }

    /// <summary>
    /// دریافت خلاصه حضور و غیاب دانشجو در تمام دوره‌ها
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <returns>لیست خلاصه حضور و غیاب در هر دوره</returns>
    /// <response code="200">خلاصه حضور دانشجو با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{studentId}/attendance-summary")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentAttendanceSummary(string studentId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            var query = new GetStudentAttendanceSummaryQuery { StudentId = studentId.Trim() };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return SuccessResponse(new List<object>(), "هیچ اطلاعات حضوری برای این دانشجو یافت نشد");

            return SuccessResponse(result, "خلاصه حضور دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت خلاصه حضور دانشجو");
    }
}