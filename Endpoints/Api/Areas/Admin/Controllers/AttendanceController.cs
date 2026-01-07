using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Attendance;
using Pardis.Domain.Attendance;
using Pardis.Domain.Users;
using Pardis.Query.Attendance;
using System.ComponentModel.DataAnnotations;

namespace Endpoints.Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت حضور و غیاب - فقط برای ادمین و مدرس
/// </summary>
[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Policy = Policies.CommentManagement.Access)]
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
            // اعتبارسنجی پارامترهای ورودی
            if (request.CourseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره الزامی است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            if (string.IsNullOrWhiteSpace(request.Title))
                return ValidationErrorResponse("عنوان جلسه الزامی است", new { title = "عنوان جلسه نمی‌تواند خالی باشد" });

            if (request.Duration <= TimeSpan.Zero)
                return ValidationErrorResponse("مدت زمان جلسه نامعتبر است", new { duration = "مدت زمان باید مثبت باشد" });

            if (request.SessionNumber <= 0)
                return ValidationErrorResponse("شماره جلسه نامعتبر است", new { sessionNumber = "شماره جلسه باید مثبت باشد" });

            var command = new CreateSessionCommand
            {
                CourseId = request.CourseId,
                ScheduleId = request.ScheduleId,
                Title = request.Title.Trim(),
                SessionDate = request.SessionDate,
                Duration = request.Duration,
                SessionNumber = request.SessionNumber
            };

            var result = await _mediator.Send(command);
            
            if (result == null)
                return ErrorResponse("خطا در ایجاد جلسه", 500, "CREATE_SESSION_FAILED");
            
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
            // اعتبارسنجی پارامترهای ورودی
            if (sessionId == Guid.Empty)
                return ValidationErrorResponse("شناسه جلسه نامعتبر است", new { sessionId = "شناسه جلسه نمی‌تواند خالی باشد" });

            if (string.IsNullOrWhiteSpace(request.Title))
                return ValidationErrorResponse("عنوان جلسه الزامی است", new { title = "عنوان جلسه نمی‌تواند خالی باشد" });

            if (request.Duration <= TimeSpan.Zero)
                return ValidationErrorResponse("مدت زمان جلسه نامعتبر است", new { duration = "مدت زمان باید مثبت باشد" });

            var command = new UpdateSessionCommand
            {
                SessionId = sessionId,
                Title = request.Title.Trim(),
                SessionDate = request.SessionDate,
                Duration = request.Duration
            };

            var result = await _mediator.Send(command);
            
            if (result == null)
                return NotFoundResponse("جلسه یافت نشد");
            
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
            // اعتبارسنجی پارامترهای ورودی
            if (sessionId == Guid.Empty)
                return ValidationErrorResponse("شناسه جلسه نامعتبر است", new { sessionId = "شناسه جلسه نمی‌تواند خالی باشد" });

            var command = new DeleteSessionCommand { SessionId = sessionId };
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFoundResponse("جلسه یافت نشد یا قابل حذف نیست");
                
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
            // اعتبارسنجی پارامترهای ورودی
            if (courseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            var query = new GetCourseSessionsQuery { CourseId = courseId };
            var result = await _mediator.Send(query);
            
            if (result == null || result.Count == 0)
                return SuccessResponse(new List<object>(), "هیچ جلسه‌ای برای این دوره یافت نشد");
            
            return SuccessResponse(result, $"{result.Count} جلسه برای این دوره یافت شد");
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
                return ValidationErrorResponse("شناسه زمان‌بندی نامعتبر است", new { scheduleId = "شناسه زمان‌بندی نمی‌تواند خالی باشد" });

            var query = new GetSessionsByScheduleQuery { ScheduleId = scheduleId };
            var result = await _mediator.Send(query);
            
            if (result == null || result.Count == 0)
                return SuccessResponse(new List<object>(), "هیچ جلسه‌ای برای این زمان‌بندی یافت نشد");
            
            return SuccessResponse(result, $"{result.Count} جلسه برای این زمان‌بندی یافت شد");
        }, "خطا در دریافت جلسات زمان‌بندی");
    }

    /// <summary>
    /// ثبت حضور و غیاب برای جلسه
    /// </summary>
    /// <param name="sessionId">شناسه جلسه</param>
    /// <param name="request">اطلاعات حضور و غیاب</param>
    /// <returns>نتیجه ثبت حضور و غیاب</returns>
    /// <response code="200">حضور و غیاب با موفقیت ثبت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و مدرس</response>
    /// <response code="404">جلسه یا دانشجو یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("session/{sessionId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RecordAttendance(Guid sessionId, [FromBody] RecordAttendanceRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // اعتبارسنجی پارامترهای ورودی
            if (sessionId == Guid.Empty)
                return ValidationErrorResponse("شناسه جلسه نامعتبر است", new { sessionId = "شناسه جلسه نمی‌تواند خالی باشد" });

            if (string.IsNullOrWhiteSpace(request.StudentId))
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            if (string.IsNullOrWhiteSpace(request.Status))
                return ValidationErrorResponse("وضعیت حضور الزامی است", new { status = "وضعیت حضور نمی‌تواند خالی باشد" });

            // بررسی معتبر بودن وضعیت حضور
            if (!Enum.TryParse<AttendanceStatus>(request.Status, true, out var attendanceStatus))
                return ValidationErrorResponse("وضعیت حضور نامعتبر است", new { 
                    status = "مقادیر مجاز: Present, Absent, Late",
                    allowedValues = new[] { "Present", "Absent", "Late" }
                });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");
            
            var command = new RecordAttendanceCommand
            {
                SessionId = sessionId,
                StudentId = request.StudentId.Trim(),
                Status = attendanceStatus,
                CheckInTime = request.CheckInTime,
                Note = request.Note?.Trim(),
                RecordedByUserId = userId
            };
            
            var result = await _mediator.Send(command);
            
            if (result == null)
                return ErrorResponse("خطا در ثبت حضور و غیاب", 500, "RECORD_ATTENDANCE_FAILED");
            
            return SuccessResponse(result, "حضور و غیاب با موفقیت ثبت شد");
        }, "خطا در ثبت حضور و غیاب");
    }

    /// <summary>
    /// بروزرسانی حضور و غیاب
    /// </summary>
    /// <param name="attendanceId">شناسه حضور و غیاب</param>
    /// <param name="request">اطلاعات بروزرسانی</param>
    /// <returns>نتیجه بروزرسانی حضور و غیاب</returns>
    /// <response code="200">حضور و غیاب با موفقیت بروزرسانی شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="401">عدم احراز هویت</response>
    /// <response code="403">عدم دسترسی - فقط ادمین و مدرس</response>
    /// <response code="404">حضور و غیاب یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPut("{attendanceId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> UpdateAttendance(Guid attendanceId, [FromBody] UpdateAttendanceRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // اعتبارسنجی پارامترهای ورودی
            if (attendanceId == Guid.Empty)
                return ValidationErrorResponse("شناسه حضور و غیاب نامعتبر است", new { attendanceId = "شناسه نمی‌تواند خالی باشد" });

            if (string.IsNullOrWhiteSpace(request.Status))
                return ValidationErrorResponse("وضعیت حضور الزامی است", new { status = "وضعیت حضور نمی‌تواند خالی باشد" });

            // بررسی معتبر بودن وضعیت حضور
            if (!Enum.TryParse<AttendanceStatus>(request.Status, true, out var attendanceStatus))
                return ValidationErrorResponse("وضعیت حضور نامعتبر است", new { 
                    status = "مقادیر مجاز: Present, Absent, Late",
                    allowedValues = new[] { "Present", "Absent", "Late" }
                });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            var command = new UpdateAttendanceCommand
            {
                AttendanceId = attendanceId,
                Status = attendanceStatus,
                CheckInTime = request.CheckInTime,
                Note = request.Note,
                UpdatedByUserId = userId
            };
            
            var result = await _mediator.Send(command);
            
            if (result == null)
                return NotFoundResponse("حضور و غیاب یافت نشد");
            
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
                return ValidationErrorResponse("شناسه جلسه نامعتبر است", new { sessionId = "شناسه جلسه نمی‌تواند خالی باشد" });

            var query = new GetSessionAttendanceQuery { SessionId = sessionId };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFoundResponse("جلسه یافت نشد");
            
            var message = result.Attendances.Any() 
                ? $"حضور و غیاب {result.Attendances.Count} دانشجو در جلسه دریافت شد"
                : "هیچ حضور و غیابی برای این جلسه ثبت نشده است";
            
            return SuccessResponse(result, message);
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
                return ValidationErrorResponse("شناسه دانشجو الزامی است", new { studentId = "شناسه دانشجو نمی‌تواند خالی باشد" });

            if (courseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            var query = new GetStudentAttendanceReportQuery(studentId.Trim(), courseId);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFoundResponse("گزارش حضور برای این دانشجو و دوره یافت نشد");
            
            var message = result.TotalSessions > 0 
                ? $"گزارش حضور دانشجو در {result.TotalSessions} جلسه دریافت شد - درصد حضور: {result.AttendancePercentage:F1}%"
                : "هیچ جلسه‌ای برای این دانشجو و دوره یافت نشد";
            
            return SuccessResponse(result, message);
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
    [Required(ErrorMessage = "شناسه دانشجو الزامی است")]
    public string StudentId { get; set; } = string.Empty;
    
    /// <summary>
    /// وضعیت حضور (Present, Absent, Late)
    /// </summary>
    [Required(ErrorMessage = "وضعیت حضور الزامی است")]
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