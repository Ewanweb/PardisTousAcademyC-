using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Comments;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;
using Pardis.Domain.Users;
using Pardis.Query.Comments;
using Api.Controllers;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت کامنت‌های دوره‌ها
/// </summary>
[Route("api/admin/comments")]
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
public class CommentsController : BaseController
{
    private readonly IMediator _mediator;

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
            var query = new GetCourseCommentsQuery
            {
                CourseId = courseId,
                Status = status,
                Page = page,
                PageSize = pageSize,
                IncludeRejected = includeRejected
            };

            var comments = await _mediator.Send(query);
            return SuccessResponse(comments, "کامنت‌ها با موفقیت دریافت شدند");
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
            var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
                return Unauthorized(new { success = false, message = "کاربر احراز هویت نشده است" });

            var command = new ReviewCommentCommand
            {
                CommentId = commentId,
                AdminUserId = adminUserId,
                Status = dto.Status,
                Note = dto.Note
            };

            var result = await _mediator.Send(command);
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
            var query = new GetCourseCommentStatsQuery { CourseId = courseId };
            var stats = await _mediator.Send(query);
            return SuccessResponse(stats, "آمار کامنت‌ها با موفقیت دریافت شد");
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
            var query = new GetPendingCommentsQuery
            {
                Page = page,
                PageSize = pageSize
            };

            var comments = await _mediator.Send(query);
            return SuccessResponse(comments, "کامنت‌های در انتظار تأیید با موفقیت دریافت شدند");
        }, "خطا در دریافت کامنت‌های در انتظار تأیید");
    }
}