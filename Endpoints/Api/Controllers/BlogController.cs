using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Blog.Posts.IncrementView;
using Pardis.Query.Blog.GetCategories;
using Pardis.Query.Blog.GetPostBySlug;
using Pardis.Query.Blog.GetPostNavigation;
using Pardis.Query.Blog.GetPostsByCategory;
using Pardis.Query.Blog.GetPostsByTag;
using Pardis.Query.Blog.GetPostsList;
using Pardis.Query.Blog.GetRelatedPosts;
using Pardis.Query.Blog.GetTags;
using Pardis.Query.Blog.SearchPosts;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/blog")]
[Route("blog")]
public class BlogController : BaseController
{
    public BlogController(IMediator mediator, ILogger<BlogController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string sort = "newest",
        [FromQuery] string? category = null,
        [FromQuery] string? tag = null,
        [FromQuery] string? q = null)
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetPostsListQuery
            {
                Page = page,
                PageSize = pageSize,
                Sort = sort,
                CategorySlug = category,
                TagSlug = tag,
                Q = q
            };
            var result = await Mediator.Send(query);
            return SuccessResponse(result);
        }, "خطا در دریافت لیست پست‌ها");
    }

    [HttpGet("posts/{slug}")]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostBySlugQuery { Slug = slug });
            if (result.Post == null && result.IsRedirect)
                return SuccessResponse(result, "Redirect");
            if (result.Post == null)
                return NotFoundResponse("مطلب یافت نشد");
            return SuccessResponse(result);
        }, "خطا در دریافت مطلب");
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetBlogCategoriesQuery());
            return SuccessResponse(result);
        }, "خطا در دریافت دسته‌بندی‌ها");
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetTagsQuery());
            return SuccessResponse(result);
        }, "خطا در دریافت تگ‌ها");
    }

    [HttpGet("category/{slug}/posts")]
    public async Task<IActionResult> GetPostsByCategory(string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostsByCategorySlugQuery
            {
                Slug = slug,
                Page = page,
                PageSize = pageSize
            });
            return SuccessResponse(result);
        }, "خطا در دریافت پست‌های دسته‌بندی");
    }

    [HttpGet("tag/{slug}/posts")]
    public async Task<IActionResult> GetPostsByTag(string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostsByTagSlugQuery
            {
                Slug = slug,
                Page = page,
                PageSize = pageSize
            });
            return SuccessResponse(result);
        }, "خطا در دریافت پست‌های تگ");
    }

    [HttpGet("posts/{slug}/related")]
    public async Task<IActionResult> GetRelated(string slug, [FromQuery] int take = 6)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetRelatedPostsQuery { Slug = slug, Take = take });
            return SuccessResponse(result);
        }, "خطا در دریافت پست‌های مرتبط");
    }

    [HttpGet("posts/{slug}/nav")]
    public async Task<IActionResult> GetNavigation(string slug)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new GetPostNavigationQuery { Slug = slug });
            return SuccessResponse(result);
        }, "خطا در دریافت ناوبری پست");
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new SearchPostsQuery { Q = q, Page = page, PageSize = pageSize });
            return SuccessResponse(result);
        }, "خطا در جستجوی پست‌ها");
    }

    [HttpPost("posts/{slug}/view")]
    public async Task<IActionResult> IncrementView(string slug)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Mediator.Send(new IncrementPostViewCommand(slug));
            return HandleOperationResult(result);
        }, "خطا در ثبت بازدید");
    }
}
