using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Dashboard;

namespace Api.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // تمام منطق محاسبه درصد رشد و ترکیب لیست‌ها در هندلر زیر انجام می‌شود
            var result = await _mediator.Send(new DashboardStatsCommand());

            return Ok(new
            {
                stats = result.Stats, // آبجکت شامل percentages
                recent_activity = result.RecentActivity // لیست ترکیبی مرتب شده
            });
        }
    }
}

