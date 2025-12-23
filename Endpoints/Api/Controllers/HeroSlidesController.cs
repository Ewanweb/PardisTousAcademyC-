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
        [HttpGet]
        public async Task<IActionResult> GetHeroSlides([FromQuery] bool includeInactive = false, [FromQuery] bool includeExpired = false, [FromQuery] bool adminView = false)
        {
            var query = new GetHeroSlidesQuery
            {
                IncludeInactive = includeInactive,
                IncludeExpired = includeExpired,
                AdminView = adminView
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// دریافت اسلاید اصلی با شناسه
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetHeroSlideById(Guid id)
        {
            var query = new GetHeroSlideByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound("اسلاید یافت نشد");

            return Ok(result);
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
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveHeroSlides()
        {
            var query = new GetHeroSlidesQuery
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