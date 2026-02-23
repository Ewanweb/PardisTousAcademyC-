using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Query.Courses.Schedules;
using Api.Controllers;
using Pardis.Application.Courses.Enroll;
using Pardis.Application.Courses.Schedules;
using Pardis.Application._Shared;
using Pardis.Domain.Users;

namespace Endpoints.Api.Controllers;

/// <summary>
/// کنترلر مدیریت زمان‌بندی‌های دوره
/// </summary>
[Route("api/courses")]
[ApiController]
[Authorize]
[Produces("application/json")]
[Tags("Course Schedules")]
public class CourseScheduleController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر زمان‌بندی‌های دوره
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    public CourseScheduleController(IMediator mediator, ILogger<CourseScheduleController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت زمان‌بندی‌های یک دوره
    /// </summary>
    /// <param name="courseId">شناسه دوره</param>
    /// <returns>لیست زمان‌بندی‌های دوره</returns>
    /// <response code="200">زمان‌بندی‌ها با موفقیت دریافت شدند</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="404">دوره یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{courseId}/schedules")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetCourseSchedules(Guid courseId)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دوره نامعتبر است" 
                });

            var query = new GetCourseSchedulesQuery(courseId);
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "زمان‌بندی‌های دوره با موفقیت دریافت شدند");
        }, "خطا در دریافت زمان‌بندی‌های دوره");
    }

    /// <summary>
    /// دریافت دانشجویان یک زمان‌بندی خاص
    /// </summary>
    /// <param name="courseId">شناسه دوره</param>
    /// <param name="scheduleId">شناسه زمان‌بندی</param>
    /// <returns>لیست دانشجویان زمان‌بندی</returns>
    /// <response code="200">دانشجویان با موفقیت دریافت شدند</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="404">دوره یا زمان‌بندی یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("{courseId}/schedules/{scheduleId}/students")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetScheduleStudents(Guid courseId, Guid scheduleId)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دوره نامعتبر است" 
                });

            if (scheduleId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه زمان‌بندی نامعتبر است" 
                });

            var query = new GetScheduleStudentsQuery(scheduleId);
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "دانشجویان زمان‌بندی با موفقیت دریافت شدند");
        }, "خطا در دریافت دانشجویان زمان‌بندی");
    }

    /// <summary>
    /// ایجاد زمان‌بندی جدید برای دوره
    /// </summary>
    /// <param name="courseId">شناسه دوره</param>
    /// <param name="request">اطلاعات زمان‌بندی</param>
    /// <returns>شناسه زمان‌بندی ایجاد شده</returns>
    /// <response code="201">زمان‌بندی با موفقیت ایجاد شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی</response>
    /// <response code="404">دوره یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("{courseId}/schedules")]
    [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> CreateSchedule(Guid courseId, [FromBody] CreateScheduleRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            if (request == null)
                return ValidationErrorResponse("اطلاعات زمان‌بندی الزامی است");

            if (string.IsNullOrWhiteSpace(request.Title))
                return ValidationErrorResponse("عنوان زمان‌بندی الزامی است", new { title = "عنوان نمی‌تواند خالی باشد" });

            if (request.MaxCapacity <= 0)
                return ValidationErrorResponse("ظرفیت نامعتبر است", new { maxCapacity = "ظرفیت باید بیشتر از صفر باشد" });

            var command = new CreateScheduleCommand
            {
                CourseId = courseId,
                Title = request.Title,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                MaxCapacity = request.MaxCapacity,
                Description = request.Description
            };

            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.Success)
            {
                return StatusCode(201, new
                {
                    success = true,
                    message = "زمان‌بندی با موفقیت ایجاد شد",
                    data = new { id = result.Data },
                    timestamp = DateTime.UtcNow
                });
            }

            if (result.Status == OperationResultStatus.NotFound)
                return NotFoundResponse(result.Message ?? "دوره یافت نشد");

            if (result.Status == OperationResultStatus.Error)
                return ErrorResponse(result.Message ?? "خطا در ایجاد زمان‌بندی", 400, "CREATE_SCHEDULE_FAILED");

            return HandleOperationResult(result);
        }, "خطا در ایجاد زمان‌بندی");
    }

    /// <summary>
    /// ثبت‌نام دستی یک کاربر در زمان‌بندی خاص (توسط ادمین)
    /// </summary>
    /// <param name="courseId">شناسه دوره</param>
    /// <param name="scheduleId">شناسه زمان‌بندی</param>
    /// <param name="request">اطلاعات ثبت‌نام</param>
    /// <returns>نتیجه ثبت‌نام</returns>
    /// <response code="200">ثبت‌نام با موفقیت انجام شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی</response>
    /// <response code="404">دوره یا زمان‌بندی یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("{courseId}/schedules/{scheduleId}/enroll")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> EnrollStudentToSchedule(
        Guid courseId, 
        Guid scheduleId, 
        [FromBody] EnrollStudentRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دوره نامعتبر است" 
                });

            if (scheduleId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه زمان‌بندی نامعتبر است" 
                });

            if (string.IsNullOrEmpty(request?.UserId))
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه کاربر الزامی است" 
                });

            var command = new EnrollUserCommand
            {
                UserId = request.UserId,
                CourseId = courseId
            };

            var result = await _mediator.Send(command);
            
            if (result.Status == OperationResultStatus.Success)
                return SuccessResponse(result.Data, "دانشجو با موفقیت در دوره ثبت‌نام شد");
            
            if (result.Status == OperationResultStatus.NotFound)
                return NotFoundResponse("دوره یا کاربر یافت نشد");
            
            if (result.Status == OperationResultStatus.Error)
                return ErrorResponse(result.Message ?? "خطا در ثبت‌نام", 400, "ENROLLMENT_FAILED");

            return HandleOperationResult(result);
        }, "خطا در ثبت‌نام دانشجو");
    }
}

/// <summary>
/// مدل درخواست ایجاد زمان‌بندی
/// </summary>
public class CreateScheduleRequest
{
    /// <summary>
    /// عنوان زمان‌بندی (مثل "گروه صبح" یا "گروه عصر")
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// روز هفته (0=یکشنبه, 1=دوشنبه, ..., 6=شنبه)
    /// </summary>
    public int DayOfWeek { get; set; }

    /// <summary>
    /// ساعت شروع (فرمت: HH:mm مثل "12:00")
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// ساعت پایان (فرمت: HH:mm مثل "14:00")
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// حداکثر ظرفیت دانشجو
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// توضیحات اضافی (اختیاری)
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// مدل درخواست ثبت‌نام دانشجو
/// </summary>
public class EnrollStudentRequest
{
    /// <summary>
    /// شناسه کاربر
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}