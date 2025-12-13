using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Courses.Schedules;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Users;
using Pardis.Query.Courses.Schedules;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/course/{courseId}/schedule")]
[ApiController]
public class CourseScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ایجاد زمان‌بندی جدید برای دوره
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
    public async Task<IActionResult> CreateSchedule(Guid courseId, [FromBody] CreateCourseScheduleDto dto)
    {
        dto.CourseId = courseId;
        var command = new CreateScheduleCommand(dto);
        var result = await _mediator.Send(command);

        if (result.Status != OperationResultStatus.Success)
            return BadRequest(new { message = result.Message });

        return CreatedAtAction(nameof(GetScheduleStudents), 
            new { courseId, scheduleId = result.Data!.Id }, 
            new { message = "زمان‌بندی با موفقیت ایجاد شد", data = result.Data });
    }

    /// <summary>
    /// ثبت‌نام دانشجو در زمان‌بندی خاص
    /// </summary>
    [HttpPost("{scheduleId}/enroll")]
    [Authorize]
    public async Task<IActionResult> EnrollInSchedule(Guid courseId, Guid scheduleId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new EnrollInScheduleCommand(scheduleId, userId);
        var result = await _mediator.Send(command);

        if (result.Status != OperationResultStatus.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// دریافت لیست دانشجویان یک زمان‌بندی
    /// </summary>
    [HttpGet("{scheduleId}/students")]
    [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
    public async Task<IActionResult> GetScheduleStudents(Guid courseId, Guid scheduleId)
    {
        var query = new GetScheduleStudentsQuery(scheduleId);
        var result = await _mediator.Send(query);

        return Ok(new { data = result });
    }
}