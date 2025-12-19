using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Domain.Attendance;
using Pardis.Query.Attendance;
using Pardis.Application.Attendance;
using Api.Controllers;

namespace Endpoints.Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت حضور و غیاب - فقط برای ادمین و مدرس
/// </summary>
[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = Role.Admin + "," + Role.Instructor)]
[Produces("application/json")]
[Tags("Attendance Management")]
public class AttendanceController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر مدیریت حضور و غیاب
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    public AttendanceController(IMediator mediator, ILogger<AttendanceController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ایجاد جلسه جدید
    /// </summary>
    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var command = new CreateSessionCommand
            {
                CourseId = request.CourseId,
                ScheduleId = request.ScheduleId,
                Title = request.Title,
                SessionDate = request.SessionDate,
                Duration = request.Duration,
                SessionNumber = request.SessionNumber
            };

            var result = await _mediator.Send(command);
            return SuccessResponse(result, "جلسه با موفقیت ایجاد شد");
        }, "خطا در ایجاد جلسه");
    }

    /// <summary>
    /// بروزرسانی جلسه
    /// </summary>
    [HttpPut("sessions/{sessionId}")]
    public async Task<IActionResult> UpdateSession(Guid sessionId, [FromBody] UpdateSessionRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var command = new UpdateSessionCommand
            {
                SessionId = sessionId,
                Title = request.Title,
                SessionDate = request.SessionDate,
                Duration = request.Duration
            };

            var result = await _mediator.Send(command);
            return SuccessResponse(result, "جلسه با موفقیت بروزرسانی شد");
        }, "خطا در بروزرسانی جلسه");
    }

    /// <summary>
    /// حذف جلسه
    /// </summary>
    [HttpDelete("sessions/{sessionId}")]
    public async Task<IActionResult> DeleteSession(Guid sessionId)
    {
        return await ExecuteAsync(async () =>
        {
            var command = new DeleteSessionCommand { SessionId = sessionId };
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { success = false, message = "جلسه یافت نشد" });
                
            return SuccessResponse(new { id = sessionId }, "جلسه با موفقیت حذف شد");
        }, "خطا در حذف جلسه");
    }

    /// <summary>
    /// دریافت جلسات یک دوره
    /// </summary>
    [HttpGet("sessions/course/{courseId}")]
    public async Task<IActionResult> GetCourseSessions(Guid courseId)
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetCourseSessionsQuery { CourseId = courseId };
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "جلسات دوره با موفقیت دریافت شدند");
        }, "خطا در دریافت جلسات دوره");
    }

    /// <summary>
    /// دریافت جلسات یک زمان‌بندی خاص
    /// </summary>
    /// <param name="scheduleId">شناسه زمان‌بندی</param>
    /// <returns>لیست جلسات زمان‌بندی</returns>
    /// <response code="200">جلسات زمان‌بندی با موفقیت دریافت شدند</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و مدرس</response>
    /// <response code="404">زمان‌بندی یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("sessions/schedule/{scheduleId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetSessionsBySchedule(Guid scheduleId)
    {
        return await ExecuteAsync(async () =>
        {
            if (scheduleId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه زمان‌بندی نامعتبر است" 
                });

            var query = new GetSessionsByScheduleQuery { ScheduleId = scheduleId };
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "جلسات زمان‌بندی با موفقیت دریافت شدند");
        }, "خطا در دریافت جلسات زمان‌بندی");
    }

    /// <summary>
    /// ثبت حضور و غیاب برای جلسه
    /// </summary>
    [HttpPost("session/{sessionId}")]
    public async Task<IActionResult> RecordAttendance(Guid sessionId, [FromBody] RecordAttendanceRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            
            var command = new RecordAttendanceCommand
            {
                SessionId = sessionId,
                StudentId = request.StudentId,
                Status = Enum.Parse<AttendanceStatus>(request.Status, true),
                CheckInTime = request.CheckInTime,
                Note = request.Note,
                RecordedByUserId = userId
            };
            
            var result = await _mediator.Send(command);
            return SuccessResponse(result, "حضور و غیاب با موفقیت ثبت شد");
        }, "خطا در ثبت حضور و غیاب");
    }

    /// <summary>
    /// بروزرسانی حضور و غیاب
    /// </summary>
    [HttpPut("{attendanceId}")]
    public async Task<IActionResult> UpdateAttendance(Guid attendanceId, [FromBody] UpdateAttendanceRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var command = new UpdateAttendanceCommand
            {
                AttendanceId = attendanceId,
                Status = Enum.Parse<AttendanceStatus>(request.Status, true),
                CheckInTime = request.CheckInTime,
                Note = request.Note
            };
            
            var result = await _mediator.Send(command);
            return SuccessResponse(result, "حضور و غیاب با موفقیت بروزرسانی شد");
        }, "خطا در بروزرسانی حضور و غیاب");
    }





    /// <summary>
    /// دریافت حضور و غیاب یک جلسه شامل لیست دانشجویان
    /// </summary>
    /// <param name="sessionId">شناسه جلسه</param>
    /// <returns>اطلاعات جلسه و حضور و غیاب دانشجویان</returns>
    /// <response code="200">حضور و غیاب جلسه با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="404">جلسه یافت نشد</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و مدرس</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("session/{sessionId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetSessionAttendance(Guid sessionId)
    {
        return await ExecuteAsync(async () =>
        {
            if (sessionId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه جلسه نامعتبر است" 
                });

            var query = new GetSessionAttendanceQuery { SessionId = sessionId };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(new { 
                    success = false, 
                    message = "جلسه یافت نشد" 
                });
            
            return SuccessResponse(result, "حضور و غیاب جلسه با موفقیت دریافت شد");
        }, "خطا در دریافت حضور و غیاب جلسه");
    }

    /// <summary>
    /// دریافت گزارش حضور دانشجو در یک دوره خاص
    /// </summary>
    /// <param name="studentId">شناسه دانشجو</param>
    /// <param name="courseId">شناسه دوره</param>
    /// <returns>گزارش کامل حضور دانشجو در دوره</returns>
    /// <response code="200">گزارش حضور دانشجو با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="404">دانشجو یا دوره یافت نشد</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و مدرس</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("student/{studentId}/course/{courseId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetStudentCourseAttendance(string studentId, Guid courseId)
    {
        return await ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دانشجو الزامی است" 
                });

            if (courseId == Guid.Empty)
                return BadRequest(new { 
                    success = false, 
                    message = "شناسه دوره نامعتبر است" 
                });

            var query = new GetStudentAttendanceReportQuery(studentId, courseId);
            var result = await _mediator.Send(query);
            
            return SuccessResponse(result, "گزارش حضور دانشجو با موفقیت دریافت شد");
        }, "خطا در دریافت گزارش حضور دانشجو");
    }
}

// Request DTOs
/// <summary>
/// درخواست ایجاد جلسه جدید
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// شناسه دوره
    /// </summary>
    public Guid CourseId { get; set; }
    
    /// <summary>
    /// شناسه زمان‌بندی (اختیاری)
    /// </summary>
    public Guid? ScheduleId { get; set; }
    
    /// <summary>
    /// عنوان جلسه
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// تاریخ و زمان برگزاری جلسه
    /// </summary>
    public DateTime SessionDate { get; set; }
    
    /// <summary>
    /// مدت زمان جلسه
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// شماره جلسه
    /// </summary>
    public int SessionNumber { get; set; }
}

/// <summary>
/// درخواست بروزرسانی جلسه
/// </summary>
public class UpdateSessionRequest
{
    /// <summary>
    /// عنوان جلسه
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// تاریخ و زمان برگزاری جلسه
    /// </summary>
    public DateTime SessionDate { get; set; }
    
    /// <summary>
    /// مدت زمان جلسه
    /// </summary>
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// درخواست ثبت حضور و غیاب
/// </summary>
public class RecordAttendanceRequest
{
    /// <summary>
    /// شناسه دانشجو
    /// </summary>
    public string StudentId { get; set; } = string.Empty;
    
    /// <summary>
    /// وضعیت حضور (Present, Absent, Late)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// زمان ورود (اختیاری)
    /// </summary>
    public DateTime? CheckInTime { get; set; }
    
    /// <summary>
    /// یادداشت (اختیاری)
    /// </summary>
    public string? Note { get; set; }
}

/// <summary>
/// درخواست بروزرسانی حضور و غیاب
/// </summary>
public class UpdateAttendanceRequest
{
    /// <summary>
    /// وضعیت حضور (Present, Absent, Late)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// زمان ورود (اختیاری)
    /// </summary>
    public DateTime? CheckInTime { get; set; }
    
    /// <summary>
    /// یادداشت (اختیاری)
    /// </summary>
    public string? Note { get; set; }
}