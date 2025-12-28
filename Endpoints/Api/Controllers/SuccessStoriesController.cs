using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Sliders.SuccessStories.Create;
using Pardis.Application.Sliders.SuccessStories.Update;
using Pardis.Application.Sliders.SuccessStories.Delete;
using Pardis.Domain.Dto.Sliders;
using Pardis.Query.Sliders.SuccessStories.GetSuccessStories;
using Pardis.Query.Sliders.SuccessStories.GetSuccessStoryById;

namespace Api.Controllers
{
    /// <summary>
    /// کنترلر مدیریت استوری‌های موفقیت - نسخه ساده‌شده
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SuccessStoriesController : BaseController
    {
        /// <summary>
        /// سازنده کنترلر استوری‌های موفقیت
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">سرویس لاگ</param>
        public SuccessStoriesController(IMediator mediator, ILogger<SuccessStoriesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// دریافت لیست استوری‌های موفقیت
        /// </summary>
        /// <param name="includeInactive">شامل استوری‌های غیرفعال</param>
        /// <returns>لیست استوری‌های موفقیت</returns>
        /// <response code="200">لیست استوری‌ها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetSuccessStories([FromQuery] bool includeInactive = false)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetSuccessStoriesQuery
                {
                    IncludeInactive = includeInactive
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
                    IncludeInactive = false
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, "استوری‌های فعال با موفقیت دریافت شد");
            }, "خطا در دریافت استوری‌های فعال");
        }

    }
}