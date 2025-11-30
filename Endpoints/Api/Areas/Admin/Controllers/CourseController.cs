using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Courses.Create;
using Pardis.Application.Courses.Update; // فرض بر وجود این نیم‌اسپیس

using Pardis.Domain.Users; // برای دسترسی به Role Constants
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Pardis.Application._Shared;
using Pardis.Application.Courses;
using Pardis.Query.Courses.GetCourses;
using Pardis.Query.Courses.GetCourseById;
using Pardis.Query.Courses.GetCoursesByCategory;
using static Pardis.Domain.Dto.Dtos;
using static Pardis.Application.Courses.SoftDeleteCommandHandler; // برای OperationResultStatus

namespace Api.Areas.Admin.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // لیست دوره‌ها (معادل index با فیلترها)
        [HttpGet()]
        public async Task<IActionResult> Index([FromQuery] GetCoursesQuery query)
        {
            // تشخیص کاربر لاگین شده برای نمایش دوره‌های Draft
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // اصلاح: حروف بزرگ برای پراپرتی‌ها
            query.CurrentUserId = userId;
            query.IsAdminOrManager = userRole == Role.Admin || userRole == Role.Manager;

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // نمایش تکی (معادل show)
        [HttpGet("{id}")]
        public async Task<IActionResult> Show(Guid id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery { Id = id });
            if (result == null) return NotFound(new { message = "دوره یافت نشد" });

            return Ok(new { data = result });
        }

        // نمایش دوره‌های یک دسته‌بندی
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> CourseCategory(Guid categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole(Role.Admin) || User.IsInRole(Role.Manager);

            var result = await _mediator.Send(new GetCoursesByCategoryQuery
            {
                CategoryId = categoryId,
                IsAdminOrManager = isAdmin
            });

            return Ok(result);
        }

        // ایجاد دوره
        [HttpPost()]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        public async Task<IActionResult> Store([FromForm] CreateCourseCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var commandWithUser = command with { CurrentUserId = userId };

            // ارسال نسخه جدید به مدیات‌ر
            var result = await _mediator.Send(commandWithUser);

            if (result.Status == OperationResultStatus.Error)
                return BadRequest(result.Message);

            return StatusCode(201, new { message = "دوره با موفقیت ایجاد شد.", data = result.Data });
        }

        // ویرایش دوره
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager + "," + Role.Instructor)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCourseCommand command)
        {
            command.Id = id;
            command.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // چک کردن اینکه آیا کاربر ادمین است (برای اجازه تغییر مدرس)
            // دقت کنید که پراپرتی IsAdmin باید در کامند UpdateCourseCommand وجود داشته باشد
            // command.IsAdmin = User.IsInRole(Role.Admin); 

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
        [Authorize(Roles = Role.Admin)] // فقط ادمین
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _mediator.Send(new RestoreCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره بازیابی شد." });
        }

        // حذف دائم (Force Delete)
        [HttpDelete("{id}/force")]
        [Authorize(Roles = Role.Admin)] // فقط ادمین
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            var result = await _mediator.Send(new ForceDeleteCourseCommand { Id = id });

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دوره به طور کامل حذف شد." });
        }
    }
}