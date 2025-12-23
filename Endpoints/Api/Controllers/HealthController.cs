using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pardis.Infrastructure;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Test database connection
                await _context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.Now,
                    database = "Connected",
                    message = "API is working fine!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.Now,
                    database = "Disconnected",
                    error = ex.Message
                });
            }
        }
    }
}