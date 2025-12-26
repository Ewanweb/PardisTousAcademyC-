using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pardis.Infrastructure;

namespace Api.Controllers
{
    /// <summary>
    /// کنترلر بررسی سلامت سیستم
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Health Check")]
    public class HealthController : BaseController
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// سازنده کنترلر بررسی سلامت
        /// </summary>
        /// <param name="context">کانتکست دیتابیس</param>
        /// <param name="logger">لاگر</param>
        public HealthController(AppDbContext context, ILogger<HealthController> logger) : base(logger)
        {
            _context = context;
        }

        /// <summary>
        /// بررسی سلامت API و اتصال به دیتابیس
        /// </summary>
        /// <returns>وضعیت سلامت سیستم</returns>
        /// <response code="200">سیستم سالم است</response>
        /// <response code="500">مشکل در سیستم</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Get()
        {
            return await ExecuteAsync(async () =>
            {
                // Test database connection
                await _context.Database.CanConnectAsync();
                
                var healthData = new
                {
                    status = "Healthy",
                    database = "Connected",
                    message = "API is working fine!"
                };

                return SuccessResponse(healthData, "سیستم در وضعیت سالم قرار دارد");
            }, "مشکل در بررسی سلامت سیستم");
        }
    }
}