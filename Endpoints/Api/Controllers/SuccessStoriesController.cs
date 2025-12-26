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
        /// <param name="includeInactive">شامل استوری‌های غیرفعال</param>
        /// <param name="includeExpired">شامل استوری‌های منقضی</param>
        /// <param name="adminView">نمای مدیریتی</param>
        /// <param name="type">نوع استوری</param>
        /// <returns>لیست استوری‌های موفقیت</returns>
        /// <response code="200">لیست استوری‌ها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetSuccessStories([FromQuery] bool includeInactive = false, [FromQuery] bool includeExpired = false, [FromQuery] bool adminView = false, [FromQuery] string? type = null)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetSuccessStoriesQuery
                {
                    IncludeInactive = includeInactive,
                    IncludeExpired = includeExpired,
                    AdminView = adminView,
                    Type = type
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, "لیست استوری‌های موفقیت با موفقیت دریافت شد");
            }, "خطا در دریافت لیست استوری‌های موفقیت");
        }

        /// <summary>
        /// دریافت استوری موفقیت با شناسه
        /// </summary>
        /// <param name="id">شناسه استوری</param>
        /// <returns>جزئیات استوری موفقیت</returns>
        /// <response code="200">استوری با موفقیت دریافت شد</response>
        /// <response code="404">استوری یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetSuccessStoryById(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetSuccessStoryByIdQuery(id);
                var result = await Mediator.Send(query);

                if (result == null)
                    return NotFoundResponse("استوری موفقیت یافت نشد");

                return SuccessResponse(result, "استوری موفقیت با موفقیت دریافت شد");
            }, "خطا در دریافت استوری موفقیت");
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
        /// <returns>لیست استوری‌های فعال</returns>
        /// <response code="200">استوری‌های فعال با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetActiveSuccessStories()
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetSuccessStoriesQuery
                {
                    IncludeInactive = false,
                    IncludeExpired = false,
                    AdminView = false
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, "استوری‌های فعال با موفقیت دریافت شد");
            }, "خطا در دریافت استوری‌های فعال");
        }

        /// <summary>
        /// دریافت استوری‌های موفقیت بر اساس نوع
        /// </summary>
        /// <param name="type">نوع استوری</param>
        /// <returns>لیست استوری‌های موفقیت بر اساس نوع</returns>
        /// <response code="200">استوری‌ها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetSuccessStoriesByType(string type)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetSuccessStoriesQuery
                {
                    IncludeInactive = false,
                    IncludeExpired = false,
                    AdminView = false,
                    Type = type
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, $"استوری‌های نوع {type} با موفقیت دریافت شد");
            }, "خطا در دریافت استوری‌ها بر اساس نوع");
        }
    }
}