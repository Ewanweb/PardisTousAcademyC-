using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Categories.GetCategories;
using Pardis.Query.Courses.GetCourses;
using Pardis.Query.Users.GetUsersByRole;
using System.Security.Claims;

namespace Api.Controllers
{
    /// <summary>
    /// کنترلر صفحه اصلی - دریافت اطلاعات عمومی سایت
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Home")]
    public class HomeController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// سازنده کنترلر صفحه اصلی
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public HomeController(IMediator mediator, ILogger<HomeController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست مدرسین برای نمایش در صفحه اصلی
        /// </summary>
        /// <returns>لیست مدرسین</returns>
        /// <response code="200">لیست مدرسین با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("Instructors")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetInstructors()
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetUsersByRoleQuery
                {
                    Role = Role.Instructor,
                    All = true,
                    Page = 1
                };

                var result = await _mediator.Send(query);
                // ✅ بهینه‌سازی: caching برای 24 ساعت (مدرسین تغییر نمی‌کنند)
                return SuccessResponseCached(result, maxAgeSeconds: 86400, "لیست مدرسین با موفقیت دریافت شد");
            }, "خطا در دریافت لیست مدرسین");
        }

        /// <summary>
        /// دریافت لیست دسته‌بندی‌ها برای نمایش در صفحه اصلی
        /// </summary>
        /// <returns>لیست دسته‌بندی‌ها</returns>
        /// <response code="200">لیست دسته‌بندی‌ها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("Categories")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCategories()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetCategoriesQuery());
                // ✅ بهینه‌سازی: caching برای 24 ساعت (دسته‌بندی‌ها تغییر نمی‌کنند)
                return SuccessResponseCached(result, maxAgeSeconds: 86400, "لیست دسته‌بندی‌ها با موفقیت دریافت شد");
            }, "خطا در دریافت لیست دسته‌بندی‌ها");
        }

        /// <summary>
        /// دریافت لیست دوره‌ها برای نمایش در صفحه اصلی
        /// </summary>
        /// <param name="query">پارامترهای جستجو و فیلتر</param>
        /// <returns>لیست دوره‌ها</returns>
        /// <response code="200">لیست دوره‌ها با موفقیت دریافت شد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("Courses")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCourses([FromQuery] GetCoursesQuery query)
        {
            return await ExecuteAsync(async () =>
            {
                // تشخیص کاربر لاگین شده برای نمایش دوره‌های Draft
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // اصلاح: حروف بزرگ برای پراپرتی‌ها
                query.CurrentUserId = userId;
                query.IsAdminOrManager = false;
                query.IsInstructor = false;

                var result = await _mediator.Send(query);
                return SuccessResponse(result, "لیست دوره‌ها با موفقیت دریافت شد");
            }, "خطا در دریافت لیست دوره‌ها");
        }
    }
}
