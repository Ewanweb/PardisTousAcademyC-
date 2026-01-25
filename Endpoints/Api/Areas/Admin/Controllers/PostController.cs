using System.Security.Claims;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Blog.CreatePost;
using Pardis.Domain.Dto.Blog;

namespace Api.Areas.Admin.Controllers
{
    [Area("Blog")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Post Management")]
    [Produces("application/json")]
    public class PostController : BaseController
    {
        public PostController(IMediator mediator, ILogger<PostController> logger) : base(mediator, logger)
        {
        }


        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Store(PostDTO dto)
        {
            return await ExecuteAsync(async () =>
            {
                if (!ModelState.IsValid)
                    BadRequest(dto);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                dto.UserId = userId;

                var command = new CreatePostCommand(dto);

                var result = await Mediator.Send(command);

                if (result.Status != OperationResultStatus.Success)
                    BadRequest(result.Message);

                return Ok(result.Data);

            }, "خطا عیر منتظره ساعتی دیگر امتحان کنید");
        }

    }
}
