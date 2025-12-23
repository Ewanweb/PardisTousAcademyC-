using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Comments;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;
using Pardis.Domain.Users;
using Pardis.Query.Comments;
using Api.Controllers;
using Pardis.Application._Shared;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت کامنت‌های دوره‌ها
/// </summary>
[Route("api/admin/comments")]
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
public class CommentsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// سازنده کنترلر مدیریت کامنت‌ها
    /// </summary>
    /// <param name="mediator">واسط MediatR</param>
    /// <param name="logger">لاگر</param>
    public CommentsController(IMediator mediator, ILogger<CommentsController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// دریافت کامنت‌های یک دوره
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseComments(
        Guid courseId,
        [FromQuery] CommentStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeRejected = false)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            if (page < 1)
                return ValidationErrorResponse("شماره صفحه نامعتبر است", new { page = "شماره صفحه باید بزرگتر از صفر باشد" });

            if (pageSize < 1 || pageSize > 100)
                return ValidationErrorResponse("تعداد آیتم در هر صفحه نامعتبر است", new { pageSize = "تعداد آیتم باید بین 1 تا 100 باشد" });

            var query = new GetCourseCommentsQuery
            {
                CourseId = courseId,
                Status = status,
                Page = page,
                PageSize = pageSize,
                IncludeRejected = includeRejected
            };

            var comments = await _mediator.Send(query);
            
            if (comments == null)
                return SuccessResponse(new List<object>(), "هیچ کامنتی برای این دوره یافت نشد");

            return SuccessResponse(comments, "کامنت‌های دوره با موفقیت دریافت شدند");
        }, "خطا در دریافت کامنت‌ها");
    }

    /// <summary>
    /// تأیید یا رد کامنت
    /// </summary>
    [HttpPut("{commentId}/review")]
    public async Task<IActionResult> ReviewComment(Guid commentId, [FromBody] ReviewCommentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            if (commentId == Guid.Empty)
                return ValidationErrorResponse("شناسه کامنت نامعتبر است", new { commentId = "شناسه کامنت نمی‌تواند خالی باشد" });

            if (dto == null)
                return ValidationErrorResponse("اطلاعات بررسی کامنت الزامی است");

            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            var command = new ReviewCommentCommand
            {
                CommentId = commentId,
                AdminUserId = adminUserId,
                Status = dto.Status,
                Note = dto.Note?.Trim()
            };

            var result = await _mediator.Send(command);
            
            if (result.Status == OperationResultStatus.Success)
                return SuccessResponse(result.Data, "کامنت با موفقیت بررسی شد");
            
            if (result.Status == OperationResultStatus.NotFound)
                return NotFoundResponse("کامنت یافت نشد");
            
            if (result.Status == OperationResultStatus.Error)
                return ErrorResponse(result.Message ?? "خطا در بررسی کامنت", 400, "REVIEW_COMMENT_FAILED");

            return HandleOperationResult(result);
        }, "خطا در بررسی کامنت");
    }

    /// <summary>
    /// دریافت آمار کامنت‌های دوره
    /// </summary>
    [HttpGet("course/{courseId}/stats")]
    public async Task<IActionResult> GetCourseCommentStats(Guid courseId)
    {
        return await ExecuteAsync(async () =>
        {
            if (courseId == Guid.Empty)
                return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

            var query = new GetCourseCommentStatsQuery { CourseId = courseId };
            var stats = await _mediator.Send(query);
            
            if (stats == null)
                return SuccessResponse(new { totalComments = 0, approvedComments = 0, pendingComments = 0, rejectedComments = 0 }, "آمار کامنت‌های دوره");

            return SuccessResponse(stats, "آمار کامنت‌های دوره با موفقیت دریافت شد");
        }, "خطا در دریافت آمار کامنت‌ها");
    }

    /// <summary>
    /// دریافت کامنت‌های در انتظار تأیید
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingComments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            if (page < 1)
                return ValidationErrorResponse("شماره صفحه نامعتبر است", new { page = "شماره صفحه باید بزرگتر از صفر باشد" });

            if (pageSize < 1 || pageSize > 100)
                return ValidationErrorResponse("تعداد آیتم در هر صفحه نامعتبر است", new { pageSize = "تعداد آیتم باید بین 1 تا 100 باشد" });

            var query = new GetPendingCommentsQuery
            {
                Page = page,
                PageSize = pageSize
            };

            var comments = await _mediator.Send(query);
            
            if (comments == null)
                return SuccessResponse(new List<object>(), "هیچ کامنت در انتظار تأییدی وجود ندارد");

            return SuccessResponse(comments, "کامنت‌های در انتظار تأیید با موفقیت دریافت شدند");
        }, "خطا در دریافت کامنت‌های در انتظار تأیید");
    }
}