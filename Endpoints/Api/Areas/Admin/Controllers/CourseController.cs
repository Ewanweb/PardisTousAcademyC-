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

        /// <summary>
        /// سازنده کنترلر مدیریت دوره‌ها
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public CourseController(IMediator mediator, ILogger<CourseController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست دوره‌ها با قابلیت فیلتر و جستجو
        /// </summary>
        /// <param name="query">پارامترهای جستجو و فیلتر</param>
        /// <returns>لیست دوره‌ها</returns>
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

        /// <summary>
        /// دریافت اطلاعات یک دوره بر اساس slug
        /// </summary>
        /// <param name="slug">شناسه متنی دوره</param>
        /// <returns>اطلاعات کامل دوره</returns>
        [HttpGet("{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return ValidationErrorResponse("شناسه دوره الزامی است", new { slug = "شناسه دوره نمی‌تواند خالی باشد" });

                var result = await _mediator.Send(new GetCoursesBySlugQuery(slug.Trim()));
                
                if (result == null) 
                    return NotFoundResponse("دوره با این شناسه یافت نشد");

                return SuccessResponse(result, "اطلاعات دوره با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات دوره");
        }

        /// <summary>
        /// دریافت دوره‌های یک دسته‌بندی خاص
        /// </summary>
        /// <param name="slug">شناسه متنی دسته‌بندی</param>
        /// <returns>لیست دوره‌های دسته‌بندی</returns>
        [HttpGet("category/{slug}")]
        public async Task<IActionResult> CourseCategory(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return ValidationErrorResponse("شناسه دسته‌بندی الزامی است", new { slug = "شناسه دسته‌بندی نمی‌تواند خالی باشد" });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole(Role.Admin) || User.IsInRole(Role.Manager);

                var result = await _mediator.Send(new GetCoursesByCategoryQuery
                {
                    Slug = slug.Trim(),
                    IsAdminOrManager = isAdmin
                });

                if (result == null)
                    return NotFoundResponse("دسته‌بندی یافت نشد");

                // Cast کردن result به dynamic برای دسترسی به data
                dynamic dynamicResult = result;
                var courses = dynamicResult?.data as IEnumerable<object>;

                if (courses == null || !courses.Any())
                    return SuccessResponse(new { data = new List<object>(), category_info = dynamicResult?.category_info }, "هیچ دوره‌ای در این دسته‌بندی یافت نشد");

                return SuccessResponse(result, $"{courses.Count()} دوره در این دسته‌بندی یافت شد");
            }, "خطا در دریافت دوره‌های دسته‌بندی");
        }

        /// <summary>
        /// ایجاد دوره جدید
        /// </summary>
        /// <param name="dto">اطلاعات دوره جدید</param>
        /// <returns>اطلاعات دوره ایجاد شده</returns>
        [HttpPost()]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        public async Task<IActionResult> Store([FromForm] CreateCourseDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                // اعتبارسنجی پارامترهای ورودی
                if (dto == null)
                    return ValidationErrorResponse("اطلاعات دوره الزامی است");

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return ValidationErrorResponse("عنوان دوره الزامی است", new { title = "عنوان دوره نمی‌تواند خالی باشد" });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return UnauthorizedResponse("کاربر احراز هویت نشده است");

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
                        data = result.Data,
                        timestamp = DateTime.UtcNow
                    });
                }

                if (result.Status == OperationResultStatus.Error)
                {
                    return ErrorResponse(result.Message ?? "خطا در ایجاد دوره", 400, "CREATE_COURSE_FAILED");
                }

                return HandleOperationResult(result);
            }, "خطا در ایجاد دوره");
        }

        /// <summary>
        /// بروزرسانی اطلاعات دوره
        /// </summary>
        /// <param name="id">شناسه دوره</param>
        /// <param name="dto">اطلاعات جدید دوره</param>
        /// <returns>اطلاعات بروزرسانی شده دوره</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCourseDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                // اعتبارسنجی پارامترهای ورودی
                if (id == Guid.Empty)
                    return ValidationErrorResponse("شناسه دوره نامعتبر است", new { id = "شناسه دوره نمی‌تواند خالی باشد" });

                if (dto == null)
                    return ValidationErrorResponse("اطلاعات دوره الزامی است");

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return ValidationErrorResponse("عنوان دوره الزامی است", new { title = "عنوان دوره نمی‌تواند خالی باشد" });

                dto.Id = id;

                UpdateCourseCommand command = new UpdateCourseCommand()
                {
                    Dto = dto,
                };

                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.NotFound) 
                    return NotFoundResponse("دوره یافت نشد");
                
                if (result.Status == OperationResultStatus.Error) 
                    return ErrorResponse(result.Message ?? "خطا در بروزرسانی دوره", 400, "UPDATE_COURSE_FAILED");

                return SuccessResponse(result.Data, "دوره با موفقیت بروزرسانی شد");
            }, "خطا در بروزرسانی دوره");
        }

        /// <summary>
        /// حذف نرم دوره (انتقال به سطل زباله)
        /// </summary>
        /// <param name="id">شناسه دوره</param>
        /// <returns>نتیجه حذف</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Destroy(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                if (id == Guid.Empty)
                    return ValidationErrorResponse("شناسه دوره نامعتبر است", new { id = "شناسه دوره نمی‌تواند خالی باشد" });

                var command = new SoftDeleteCommand(id);
                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.NotFound) 
                    return NotFoundResponse("دوره یافت نشد");
                
                if (result.Status == OperationResultStatus.Error) 
                    return ErrorResponse(result.Message ?? "خطا در حذف دوره", 400, "DELETE_COURSE_FAILED");

                return SuccessResponse(new { id }, "دوره با موفقیت به سطل زباله منتقل شد");
            }, "خطا در حذف دوره");
        }

        /// <summary>
        /// بازیابی دوره از سطل زباله
        /// </summary>
        /// <param name="id">شناسه دوره</param>
        /// <returns>نتیجه بازیابی</returns>
        [HttpPost("{id}/restore")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _mediator.Send(new RestoreCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره بازیابی شد." });
        }

        /// <summary>
        /// حذف دائمی دوره (غیرقابل بازگشت)
        /// </summary>
        /// <param name="id">شناسه دوره</param>
        /// <returns>نتیجه حذف دائمی</returns>
        [HttpDelete("{id}/force")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            var result = await _mediator.Send(new ForceDeleteCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره به طور کامل حذف شد." });
        }


        /// <summary>
        /// ثبت‌نام کاربر در دوره (بعد از پرداخت)
        /// </summary>
        /// <param name="id">شناسه دوره</param>
        /// <returns>نتیجه ثبت‌نام</returns>
        [HttpPost("{id}/enroll")]
        [Authorize]
        public async Task<IActionResult> Enroll(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                if (id == Guid.Empty)
                    return ValidationErrorResponse("شناسه دوره نامعتبر است", new { id = "شناسه دوره نمی‌تواند خالی باشد" });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) 
                    return UnauthorizedResponse("کاربر احراز هویت نشده است");

                var command = new EnrollUserCommand
                {
                    UserId = userId,
                    CourseId = id
                };

                var result = await _mediator.Send(command);
                
                if (result.Status == OperationResultStatus.Success)
                    return SuccessResponse(result.Data, "ثبت‌نام در دوره با موفقیت انجام شد");
                
                if (result.Status == OperationResultStatus.NotFound)
                    return NotFoundResponse("دوره یافت نشد");
                
                if (result.Status == OperationResultStatus.Error)
                    return ErrorResponse(result.Message ?? "خطا در ثبت‌نام", 400, "ENROLLMENT_FAILED");

                return HandleOperationResult(result);
            }, "خطا در ثبت‌نام در دوره");
        }

        /// <summary>
        /// دریافت لیست دوره‌های خریداری شده کاربر فعلی
        /// </summary>
        /// <returns>لیست دوره‌های ثبت‌نام شده</returns>
        [HttpGet("my-enrollments")]
        [Authorize]
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
                    return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

                if (page < 1)
                    return ValidationErrorResponse("شماره صفحه نامعتبر است", new { page = "شماره صفحه باید بزرگتر از صفر باشد" });

                if (pageSize < 1 || pageSize > 100)
                    return ValidationErrorResponse("تعداد آیتم در هر صفحه نامعتبر است", new { pageSize = "تعداد آیتم باید بین 1 تا 100 باشد" });

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
                    return ValidationErrorResponse("شناسه دوره نامعتبر است", new { courseId = "شناسه دوره نمی‌تواند خالی باشد" });

                var query = new GetCourseFinancialSummaryQuery { CourseId = courseId };
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFoundResponse("دوره یافت نشد یا اطلاعات مالی در دسترس نیست");
                
                return SuccessResponse(result, "خلاصه مالی دوره با موفقیت دریافت شد");
            }, "خطا در دریافت خلاصه مالی دوره");
        }
    }
}