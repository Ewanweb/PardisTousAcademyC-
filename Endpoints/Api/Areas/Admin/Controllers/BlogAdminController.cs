using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Blog.Categories.CreateCategory;
using Pardis.Application.Blog.Categories.DeleteCategory;
using Pardis.Application.Blog.Categories.UpdateCategory;
using Pardis.Application.Blog.Posts.CreatePost;
using Pardis.Application.Blog.Posts.DeletePost;
using Pardis.Application.Blog.Posts.PublishPost;
using Pardis.Application.Blog.Posts.UpdatePost;
using Pardis.Application.Blog.Tags.CreateTag;
using Pardis.Application.Blog.Tags.DeleteTag;
using Pardis.Application.Blog.Tags.UpdateTag;
using Pardis.Application.Blog.Posts.UploadImage;
using Pardis.Domain.Dto.Blog;
using Pardis.Query.Blog.GetPostsList;
using Pardis.Query.Blog.GetPostById;

namespace Api.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/blog")]
[ApiController]
[Authorize(Policy = Policies.AdminSystem.Access)]
[Produces("application/json")]
[Tags("Admin - Blog")]
public class BlogAdminController : BaseController
{
    public BlogAdminController(IMediator mediator, ILogger<BlogAdminController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sort = "newest",
        [FromQuery] string? category = null,
        [FromQuery] string? tag = null,
        [FromQuery] string? q = null,
        [FromQuery] string? status = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostsListQuery
            {
                Page = page,
                PageSize = pageSize,
                Sort = sort,
                CategorySlug = category,
                TagSlug = tag,
                Q = q,
                Status = status,
                IncludeDrafts = true
            });
            return SuccessResponse(result);
        }, "خطا در دریافت لیست پست‌ها");
    }

    [HttpGet("posts/{id:guid}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostByIdQuery { Id = id, IncludeDrafts = true });
            if (result == null)
                return NotFound(new { message = "پست یافت نشد" });
            return SuccessResponse(result);
        }, "خطا در دریافت پست");
    }

    [HttpPost("posts")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await Mediator.Send(new CreatePostCommand(dto, userId));
            return HandleOperationResult(result);
        }, "خطا در ایجاد پست");
    }

    [HttpPut("posts/{id:guid}")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new UpdatePostCommand(id, dto, GetCurrentUserId()));
            return HandleOperationResult(result);
        }, "خطا در بروزرسانی پست");
    }

    [HttpPost("posts/{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, [FromBody] DateTime? publishedAt = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new PublishPostCommand(id, publishedAt));
            return HandleOperationResult(result);
        }, "خطا در انتشار پست");
    }

    [HttpDelete("posts/{id:guid}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new DeletePostCommand(id));
            return HandleOperationResult(result);
        }, "خطا در حذف پست");
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new CreateBlogCategoryCommand(dto, GetCurrentUserId()));
            return HandleOperationResult(result);
        }, "خطا در ایجاد دسته‌بندی");
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new UpdateBlogCategoryCommand(id, dto));
            return HandleOperationResult(result);
        }, "خطا در بروزرسانی دسته‌بندی");
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new DeleteBlogCategoryCommand(id));
            return HandleOperationResult(result);
        }, "خطا در حذف دسته‌بندی");
    }

    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new CreateTagCommand(dto));
            return HandleOperationResult(result);
        }, "خطا در ایجاد تگ");
    }

    [HttpPut("tags/{id:guid}")]
    public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateTagRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new UpdateTagCommand(id, dto));
            return HandleOperationResult(result);
        }, "خطا در بروزرسانی تگ");
    }

    [HttpDelete("tags/{id:guid}")]
    public async Task<IActionResult> DeleteTag(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new DeleteTagCommand(id));
            return HandleOperationResult(result);
        }, "خطا در حذف تگ");
    }

    [HttpPost("upload-image")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB limit
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile imageFile)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await Mediator.Send(new UploadBlogImageCommand(imageFile, userId));
            return HandleOperationResult(result);
        }, "خطا در آپلود تصویر");
    }
}
