using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Query.Courses.Schedules;
using Api.Controllers;

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
}