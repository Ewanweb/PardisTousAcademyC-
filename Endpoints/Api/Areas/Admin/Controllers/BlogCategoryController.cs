using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Blog.CreatePost;
using Pardis.Domain.Dto.Blog;
using System.Security.Claims;
using Pardis.Application.Blog.CreateBlogCategory;

namespace Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Blog Category Management")]
    [Produces("application/json")]
    public class BlogCategoryController : BaseController
    {
        
        public BlogCategoryController(IMediator mediator, ILogger<BlogCategoryController> logger) : base(mediator, logger)
        {
        }


        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Store(BlogCategoriesDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                if (!ModelState.IsValid)
                    BadRequest(dto);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                dto.UserId = userId;

                var command = new CreateBlogCategoryCommand(dto);

                var result = await Mediator.Send(command);

                if (result.Status != OperationResultStatus.Success)
                    BadRequest(result.Message);

                return Ok(result.Data);

            }, "خطا عیر منتظره ساعتی دیگر امتحان کنید");
        }
    }
}
