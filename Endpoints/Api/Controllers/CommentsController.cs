using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Comments;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;
using Pardis.Query.Comments;

namespace Api.Controllers;

/// <summary>
/// کنترلر کامنت‌های دانشجویان
/// </summary>
[Route("api/comments")]
[Authorize]
public class CommentsController : BaseController
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator, ILogger<CommentsController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ثبت کامنت جدید برای دوره
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "کاربر احراز هویت نشده است" });

            var command = new CreateCommentCommand
            {
                CourseId = dto.CourseId,
                UserId = userId,
                Content = dto.Content,
                Rating = dto.Rating
            };

            var result = await _mediator.Send(command);
            return HandleOperationResult(result);
        }, "خطا در ثبت کامنت");
    }

    /// <summary>
    /// دریافت کامنت‌های تأیید شده یک دوره
    /// </summary>
    [HttpGet("course/{courseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetApprovedCourseComments(
        Guid courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetCourseCommentsQuery
            {
                CourseId = courseId,
                Status = CommentStatus.Approved,
                Page = page,
                PageSize = pageSize
            };

            var comments = await _mediator.Send(query);
            return SuccessResponse(comments, "کامنت‌ها با موفقیت دریافت شدند");
        }, "خطا در دریافت کامنت‌ها");
    }

    /// <summary>
    /// بروزرسانی کامنت (فقط کامنت‌های در انتظار تأیید)
    /// </summary>
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "کاربر احراز هویت نشده است" });

            var command = new UpdateCommentCommand
            {
                CommentId = commentId,
                UserId = userId,
                Content = dto.Content,
                Rating = dto.Rating
            };

            var result = await _mediator.Send(command);
            return HandleOperationResult(result);
        }, "خطا در بروزرسانی کامنت");
    }
}