using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Categories.GetCategories;
using Pardis.Query.Courses.GetCourses;
using Pardis.Query.Users.GetUsersByRole;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IMediator _mediator;
        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Instructors")]
        public async Task<IActionResult> GetInstructors()
        {
            var query = new GetUsersByRoleQuery
            {
                Role = Role.Instructor,
                All = true,
                Page = 1
            };

            var result = await _mediator.Send(query);
            return Ok(new { data = result });
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return Ok(new { data = result });
        }

        [HttpGet("Courses")]
        public async Task<IActionResult> Index([FromQuery] GetCoursesQuery query)
        {
            // تشخیص کاربر لاگین شده برای نمایش دوره‌های Draft
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // اصلاح: حروف بزرگ برای پراپرتی‌ها
            query.CurrentUserId = userId;
            query.IsAdminOrManager = false;
            query.IsInstructor = false;

            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
