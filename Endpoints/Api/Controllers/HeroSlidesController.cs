using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Create;
using Pardis.Application.Sliders.HeroSlides.Delete;
using Pardis.Application.Sliders.HeroSlides.Update;
using Pardis.Domain.Dto.Sliders;
using Pardis.Query.Sliders.HeroSlides.GetHeroSlideById;
using Pardis.Query.Sliders.HeroSlides.GetHeroSlides;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeroSlidesController : BaseController
    {
        public HeroSlidesController(IMediator mediator, ILogger<HeroSlidesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// دریافت لیست اسلایدهای اصلی
        /// </summary>
        /// <param name="includeInactive">شامل اسلایدهای غیرفعال</param>
        /// <param name="includeExpired">شامل اسلایدهای منقضی</param>
        /// <param name="adminView">نمای مدیریتی</param>
        /// <returns>لیست اسلایدهای اصلی</returns>
        /// <response code="200">لیست اسلایدها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetHeroSlides([FromQuery] bool includeInactive = false, [FromQuery] bool includeExpired = false, [FromQuery] bool adminView = false)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetHeroSlidesQuery
                {
                    IncludeInactive = includeInactive,
                    IncludeExpired = includeExpired,
                    AdminView = adminView
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, "لیست اسلایدهای اصلی با موفقیت دریافت شد");
            }, "خطا در دریافت لیست اسلایدهای اصلی");
        }

        /// <summary>
        /// دریافت اسلاید اصلی با شناسه
        /// </summary>
        /// <param name="id">شناسه اسلاید</param>
        /// <returns>جزئیات اسلاید</returns>
        /// <response code="200">اسلاید با موفقیت دریافت شد</response>
        /// <response code="404">اسلاید یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetHeroSlideById(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetHeroSlideByIdQuery(id);
                var result = await Mediator.Send(query);

                if (result == null)
                    return NotFoundResponse("اسلاید یافت نشد");

                return SuccessResponse(result, "اسلاید با موفقیت دریافت شد");
            }, "خطا در دریافت اسلاید");
        }

        /// <summary>
        /// ایجاد اسلاید اصلی جدید
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateHeroSlide([FromForm] CreateHeroSlideDto dto)
        {
            var command = new CreateHeroSlideCommand(dto)
            {
                CurrentUserId = GetCurrentUserId()
            };

            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// به‌روزرسانی اسلاید اصلی
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateHeroSlide(Guid id, [FromForm] UpdateHeroSlideDto dto)
        {
            var command = new UpdateHeroSlideCommand(id, dto)
            {
                CurrentUserId = GetCurrentUserId()
            };

            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// حذف اسلاید اصلی
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteHeroSlide(Guid id)
        {
            var command = new DeleteHeroSlideCommand(id);
            var result = await Mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// دریافت اسلایدهای فعال برای نمایش عمومی
        /// </summary>
        /// <returns>لیست اسلایدهای فعال</returns>
        /// <response code="200">اسلایدهای فعال با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetActiveHeroSlides()
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetHeroSlidesQuery
                {
                    IncludeInactive = false,
                    IncludeExpired = false,
                    AdminView = false
                };

                var result = await Mediator.Send(query);
                return SuccessResponse(result, "اسلایدهای فعال با موفقیت دریافت شد");
            }, "خطا در دریافت اسلایدهای فعال");
        }
    }
}