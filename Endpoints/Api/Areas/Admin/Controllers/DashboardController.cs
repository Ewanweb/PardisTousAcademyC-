using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Dashboard;
using System.Diagnostics;

namespace Api.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.Dashboard.Access)]
    public class DashboardController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DashboardController(ILogger<DashboardController> logger, IMediator mediator, IHttpContextAccessor httpContextAccessor) : base(logger)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await ExecuteAsync(async () =>
            {

                var result = await _mediator.Send(new DashboardStatsCommand());

                return Ok(new
                {
                    success = true,
                    message = "آمار داشبورد با موفقیت دریافت شد",
                    data = new
                    {
                        stats = result.Stats,
                        recent_activity = result.RecentActivity
                    }
                });
            }, "خطا در دریافت آمار داشبورد");
        }
    }
}

