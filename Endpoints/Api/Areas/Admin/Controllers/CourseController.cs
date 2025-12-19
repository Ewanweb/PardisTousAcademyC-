using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Courses;
using Pardis.Application.Courses.Create;
using Pardis.Application.Courses.Update;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Users;
using Pardis.Query.Courses.GetCourses;
using Pardis.Query.Courses.GetCoursesByCategory;
using Pardis.Query.Courses.GetCoursesBySlug;
using Pardis.Query.Courses;
using System.Security.Claims;
using Pardis.Application.Courses.Enroll;
using Pardis.Query.Courses.Enroll;
using static Pardis.Application.Courses.SoftDeleteCommandHandler;
using Api.Controllers;

namespace Api.Areas.Admin.Controllers
{
    /// <summary>
    /// کنترلر مدیریت دوره‌ها
    /// </summary>
    [Route("api/courses")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Courses Management")]
    public class CourseController : BaseController
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator, ILogger<CourseController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        // لیست دوره‌ها (معادل index با فیلترها)
        [HttpGet()]
        public async Task<IActionResult> Index([FromQuery] GetCoursesQuery query)
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                query.CurrentUserId = userId;
                query.IsAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
                query.IsInstructor = User.IsInRole("Instructor");

                var result = await _mediator.Send(query);
                return SuccessResponse(result, "لیست دوره‌ها با موفقیت دریافت شد");
            }, "خطا در دریافت لیست دوره‌ها");
        }

        // نمایش تکی (معادل show)
        [HttpGet("{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetCoursesBySlugQuery(slug));
                
                if (result == null) 
                    return NotFound(new { 
                        success = false, 
                        message = "دوره یافت نشد" 
                    });

                return SuccessResponse(result, "اطلاعات دوره با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات دوره");
        }

        // نمایش دوره‌های یک دسته‌بندی
        [HttpGet("category/{slug}")]
        public async Task<IActionResult> CourseCategory(string slug)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole(Role.Admin) || User.IsInRole(Role.Manager);

            var result = await _mediator.Send(new GetCoursesByCategoryQuery
            {
                Slug = slug,
                IsAdminOrManager = isAdmin
            });

            return Ok(result);
        }

        // ایجاد دوره
        [HttpPost()]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        public async Task<IActionResult> Store([FromForm] CreateCourseDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                CreateCourseCommand commandWithUser = new CreateCourseCommand(dto, true)
                {
                    CurrentUserId = userId
                };

                var result = await _mediator.Send(commandWithUser);

                if (result.Status == OperationResultStatus.Success)
                {
                    return StatusCode(201, new { 
                        success = true,
                        message = "دوره با موفقیت ایجاد شد", 
                        data = result.Data 
                    });
                }

                return HandleOperationResult(result);
            }, "خطا در ایجاد دوره");
        }

        // ویرایش دوره
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCourseDto dto)
        {
            dto.Id = id;

            UpdateCourseCommand command = new UpdateCourseCommand()
            {
                Dto = dto,
            };

            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);
            if (result.Status == OperationResultStatus.Error) return BadRequest(result.Message);

            return Ok(new { message = "دوره با موفقیت ویرایش شد.", data = result.Data });
        }

        // حذف نرم (Soft Delete)
        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Destroy(Guid id)
        {
            var command = new SoftDeleteCommand(id);


            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);
            if (result.Status == OperationResultStatus.Error) return BadRequest(result.Message); // مثلا اگر دسترسی نداشت

            return Ok(new { message = "دوره به سطل زباله منتقل شد." });
        }

        // بازیابی (Restore)
        [HttpPost("{id}/restore")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)] // فقط ادمین
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _mediator.Send(new RestoreCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره بازیابی شد." });
        }

        // حذف دائم (Force Delete)
        [HttpDelete("{id}/force")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)] // فقط ادمین
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            var result = await _mediator.Send(new ForceDeleteCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره به طور کامل حذف شد." });
        }


        // ثبت‌نام کاربر در دوره (بعد از پرداخت)
        [HttpPost("{id}/enroll")]
        [Authorize]
        public async Task<IActionResult> Enroll(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) 
                    return Unauthorized(new { 
                        success = false, 
                        message = "کاربر احراز هویت نشده است" 
                    });

                var command = new EnrollUserCommand
                {
                    UserId = userId,
                    CourseId = id
                };

                var result = await _mediator.Send(command);
                return HandleOperationResult(result, "ثبت‌نام با موفقیت انجام شد");
            }, "خطا در ثبت‌نام در دوره");
        }

        // ✅ دریافت لیست دوره‌های خریداری شده من
        // GET: api/v1/courses/my-enrollments
        [HttpGet("my-enrollments")]
        [Authorize]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { 
                    success = false, 
                    message = "کاربر احراز هویت نشده است" 
                });

            var query = new GetUserEnrollmentsQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(new { data = result });
        }

        // ✅ زمان‌بندی‌های دوره حالا در CourseScheduleController مدیریت می‌شوند
        // برای دسترسی به زمان‌بندی‌ها از /api/courses/{courseId}/schedules استفاده کنید

        /// <summary>
        /// دریافت لیست دانشجویان یک دوره با قابلیت صفحه‌بندی و جستجو
        /// </summary>
        /// <param name="courseId">شناسه دوره</param>
        /// <param name="page">شماره صفحه (پیش‌فرض: 1)</param>
        /// <param name="pageSize">تعداد آیتم در هر صفحه (پیش‌فرض: 20)</param>
        /// <param name="searchTerm">عبارت جستجو در نام، ایمیل یا موبایل</param>
        /// <returns>لیست دانشجویان دوره</returns>
        /// <response code="200">لیست دانشجویان با موفقیت دریافت شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی - فقط ادمین، منیجر و مدرس</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{courseId}/students")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCourseStudents(
            [FromRoute] Guid courseId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20, 
            [FromQuery] string? searchTerm = null)
        {
            return await ExecuteAsync(async () =>
            {
                if (courseId == Guid.Empty)
                    return BadRequest(new { 
                        success = false, 
                        message = "شناسه دوره نامعتبر است" 
                    });

                if (page < 1)
                    return BadRequest(new { 
                        success = false, 
                        message = "شماره صفحه باید بزرگتر از صفر باشد" 
                    });

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest(new { 
                        success = false, 
                        message = "تعداد آیتم در هر صفحه باید بین 1 تا 100 باشد" 
                    });

                var query = new GetCourseStudentsQuery 
                { 
                    CourseId = courseId,
                    Page = page,
                    PageSize = pageSize,
                    SearchTerm = searchTerm
                };
                
                var result = await _mediator.Send(query);
                return SuccessResponse(result, "دانشجویان دوره با موفقیت دریافت شدند");
            }, "خطا در دریافت دانشجویان دوره");
        }

        /// <summary>
        /// دریافت خلاصه مالی دوره شامل درآمد، مبالغ معوق و آمار پرداخت‌ها
        /// </summary>
        /// <param name="courseId">شناسه دوره</param>
        /// <returns>خلاصه مالی دوره</returns>
        /// <response code="200">خلاصه مالی دوره با موفقیت دریافت شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="404">دوره یافت نشد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{courseId}/financial-summary")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCourseFinancialSummary([FromRoute] Guid courseId)
        {
            return await ExecuteAsync(async () =>
            {
                if (courseId == Guid.Empty)
                    return BadRequest(new { 
                        success = false, 
                        message = "شناسه دوره نامعتبر است" 
                    });

                var query = new GetCourseFinancialSummaryQuery { CourseId = courseId };
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound(new { 
                        success = false, 
                        message = "دوره یافت نشد" 
                    });
                
                return SuccessResponse(result, "خلاصه مالی دوره با موفقیت دریافت شد");
            }, "خطا در دریافت خلاصه مالی دوره");
        }
    }
}