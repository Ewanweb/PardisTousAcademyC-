using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pardis.Infrastructure;
using System.Reflection;
using System.Runtime.InteropServices;

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
                var startTime = DateTime.UtcNow;
                
                // Test database connection
                var dbConnectionTime = DateTime.UtcNow;
                var canConnect = await _context.Database.CanConnectAsync();
                var dbResponseTime = (DateTime.UtcNow - dbConnectionTime).TotalMilliseconds;

                // Get system information
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version?.ToString() ?? "Unknown";
                
                var healthData = new
                {
                    status = canConnect ? "Healthy" : "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    version = version,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    database = new
                    {
                        status = canConnect ? "Connected" : "Disconnected",
                        responseTime = $"{dbResponseTime:F2}ms"
                    },
                    system = new
                    {
                        platform = RuntimeInformation.OSDescription,
                        architecture = RuntimeInformation.OSArchitecture.ToString(),
                        framework = RuntimeInformation.FrameworkDescription,
                        uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss")
                    },
                    memory = new
                    {
                        workingSet = $"{GC.GetTotalMemory(false) / 1024 / 1024:F2} MB",
                        gcCollections = new
                        {
                            gen0 = GC.CollectionCount(0),
                            gen1 = GC.CollectionCount(1),
                            gen2 = GC.CollectionCount(2)
                        }
                    },
                    responseTime = $"{(DateTime.UtcNow - startTime).TotalMilliseconds:F2}ms"
                };

                return SuccessResponse(healthData, "سیستم در وضعیت سالم قرار دارد");
            }, "مشکل در بررسی سلامت سیستم");
        }

        /// <summary>
        /// بررسی سلامت ساده برای Load Balancer
        /// </summary>
        /// <returns>پاسخ ساده OK</returns>
        [HttpGet("ping")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// بررسی دقیق سلامت دیتابیس
        /// </summary>
        /// <returns>وضعیت دقیق دیتابیس</returns>
        [HttpGet("database")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DatabaseHealth()
        {
            return await ExecuteAsync(async () =>
            {
                var startTime = DateTime.UtcNow;
                
                // Test connection
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Test query performance
                var queryStartTime = DateTime.UtcNow;
                var userCount = await _context.Users.CountAsync();
                var queryTime = (DateTime.UtcNow - queryStartTime).TotalMilliseconds;

                var dbHealth = new
                {
                    status = canConnect ? "Healthy" : "Unhealthy",
                    connectionTest = new
                    {
                        success = canConnect,
                        responseTime = $"{connectionTime:F2}ms"
                    },
                    queryTest = new
                    {
                        success = true,
                        responseTime = $"{queryTime:F2}ms",
                        userCount = userCount
                    },
                    provider = _context.Database.ProviderName,
                    connectionString = _context.Database.GetConnectionString()?.Substring(0, 50) + "..."
                };

                return SuccessResponse(dbHealth, "وضعیت دیتابیس بررسی شد");
            }, "خطا در بررسی دیتابیس");
        }
    }
}