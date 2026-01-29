using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Query.Courses.Enroll;
using Pardis.Query.Courses.GetCourseDetail;
using System.Security.Claims;

namespace Api.Controllers
{
    /// <summary>
    /// کنترلر عمومی دوره‌ها - برای کاربران عادی
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Courses")]
    public class CoursesController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// سازنده کنترلر دوره‌ها
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public CoursesController(IMediator mediator, ILogger<CoursesController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست دوره‌های ثبت‌نام شده کاربر فعلی
        /// </summary>
        /// <returns>لیست دوره‌های ثبت‌نام شده با جزئیات کامل</returns>
        /// <response code="200">لیست دوره‌ها با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="404">هیچ دوره‌ای یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("my-enrollments")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetMyEnrollments()
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return UnauthorizedResponse("کاربر احراز هویت نشده است");

                var query = new GetUserEnrollmentsQuery { UserId = userId };
                var result = await _mediator.Send(query);

                if (result == null || result.Count == 0)
                    return SuccessResponse(new List<object>(), "شما در هیچ دوره‌ای ثبت‌نام نکرده‌اید");

                return SuccessResponse(result, $"شما در {result.Count} دوره ثبت‌نام کرده‌اید");
            }, "خطا در دریافت دوره‌های ثبت‌نام شده");
        }

        /// <summary>
        /// بررسی وضعیت ثبت‌نام کاربر در یک دوره
        /// </summary>
        /// <param name="courseId">شناسه دوره</param>
        /// <returns>وضعیت ثبت‌نام</returns>
        /// <response code="200">وضعیت ثبت‌نام با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="404">دوره یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{courseId}/enrollment-status")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetEnrollmentStatus(Guid courseId)
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return UnauthorizedResponse("کاربر احراز هویت نشده است");

                var query = new GetEnrollmentStatusQuery { UserId = userId, CourseId = courseId };
                var result = await _mediator.Send(query);

                return SuccessResponse(result, "وضعیت ثبت‌نام با موفقیت دریافت شد");
            }, "خطا در بررسی وضعیت ثبت‌نام");
        }

        /// <summary>
        /// دریافت جزئیات کامل دوره با اطلاعات دسترسی
        /// </summary>
        /// <param name="slug">Slug دوره</param>
        /// <returns>جزئیات کامل دوره</returns>
        /// <response code="200">جزئیات دوره با موفقیت دریافت شد</response>
        /// <response code="404">دوره یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("detail/{slug}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCourseDetail(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAuthenticated = !string.IsNullOrEmpty(userId);

                var query = new GetCourseDetailQuery 
                { 
                    Slug = slug,
                    UserId = userId,
                    IsAuthenticated = isAuthenticated
                };
                
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFoundResponse("دوره مورد نظر یافت نشد");

                return SuccessResponse(result, "جزئیات دوره با موفقیت دریافت شد");
            }, "خطا در دریافت جزئیات دوره");
        }
    }
}