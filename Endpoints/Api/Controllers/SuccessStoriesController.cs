using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Create;
using Pardis.Application.Sliders.SuccessStories.Update;
using Pardis.Application.Sliders.SuccessStories.Delete;
using Pardis.Domain.Dto.Sliders;
using Pardis.Query.Sliders.SuccessStories.GetSuccessStories;
using Pardis.Query.Sliders.SuccessStories.GetSuccessStoryById;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuccessStoriesController : BaseController
    {
        public SuccessStoriesController(IMediator mediator, ILogger<SuccessStoriesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// دریافت لیست استوری‌های موفقیت
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSuccessStories([FromQuery] bool includeInactive = false, [FromQuery] bool includeExpired = false, [FromQuery] bool adminView = false)
        {
            var query = new GetSuccessStoriesQuery
            {
                IncludeInactive = includeInactive,
                IncludeExpired = includeExpired,
                AdminView = adminView
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// دریافت استوری موفقیت با شناسه
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSuccessStoryById(Guid id)
        {
            var query = new GetSuccessStoryByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound("استوری موفقیت یافت نشد");

            return Ok(result);
        }

        /// <summary>
        /// ایجاد استوری موفقیت جدید
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSuccessStory([FromForm] CreateSuccessStoryDto dto)
        {
            var command = new CreateSuccessStoryCommand(dto)
            {
                CurrentUserId = GetCurrentUserId()
            };

            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// به‌روزرسانی استوری موفقیت
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateSuccessStory(Guid id, [FromForm] UpdateSuccessStoryDto dto)
        {
            var command = new UpdateSuccessStoryCommand(id, dto)
            {
                CurrentUserId = GetCurrentUserId()
            };

            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// حذف استوری موفقیت
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteSuccessStory(Guid id)
        {
            var command = new DeleteSuccessStoryCommand(id);
            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// دریافت استوری‌های فعال برای نمایش عمومی
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSuccessStories()
        {
            var query = new GetSuccessStoriesQuery
            {
                IncludeInactive = false,
                IncludeExpired = false,
                AdminView = false
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}