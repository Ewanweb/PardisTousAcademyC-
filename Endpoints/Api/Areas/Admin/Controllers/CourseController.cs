//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Pardis.Application.Courses.Create;
//using Pardis.Application.Courses.Update;
//using System.Security.Claims;

//namespace Api.Areas.Admin.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CourseController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public CourseController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        // لیست دوره‌ها (معادل index با فیلترها)
//        [HttpGet]
//        public async Task<IActionResult> Index([FromQuery] GetCoursesQuery query)
//        {
//            // تشخیص کاربر لاگین شده برای نمایش دوره‌های Draft
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

//            query.CurrentUserId = userId;
//            query.IsAdminOrManager = userRole == "Admin" || userRole == "Manager";

//            var result = await _mediator.Send(query);
//            return Ok(result); // Collection خودش دیتا و متا دیتا دارد
//        }

//        // نمایش تکی (معادل show)
//        [HttpGet("{id}")]
//        public async Task<IActionResult> Show(Guid id)
//        {
//            // Policy Check باید داخل هندلر انجام شود یا اینجا دستی چک شود
//            var result = await _mediator.Send(new GetCourseByIdQuery { Id = id });
//            return Ok(new { data = result });
//        }

//        // نمایش دوره‌های یک دسته‌بندی (Recursive)
//        [HttpGet("category/{categoryId}")]
//        public async Task<IActionResult> CourseCategory(Guid categoryId)
//        {
//            // لاجیک گرفتن تمام فرزندان و کوئری دوره‌ها در هندلر GetCoursesByCategoryQuery پیاده می‌شود
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");

//            var result = await _mediator.Send(new GetCoursesByCategoryQuery
//            {
//                CategoryId = categoryId,
//                IsAdminOrManager = isAdmin
//            });

//            return Ok(result);
//        }

//        // ایجاد دوره
//        [HttpPost]
//        [Authorize(Roles = "Admin,Manager,Instructor")]
//        public async Task<IActionResult> Store([FromForm] CreateCourseCommand command)
//        {
//            command.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var result = await _mediator.Send(command);
//            return StatusCode(201, new { message = "دوره با موفقیت ایجاد شد.", data = result });
//        }

//        // ویرایش دوره
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Admin,Manager,Instructor")]
//        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCourseCommand command)
//        {
//            command.Id = id;
//            command.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            command.IsAdmin = User.IsInRole("Admin"); // برای تغییر مدرس توسط ادمین

//            var result = await _mediator.Send(command);
//            return Ok(new { message = "دوره با موفقیت ویرایش شد.", data = result });
//        }

//        // حذف نرم (Soft Delete)
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin,Manager,Instructor")]
//        public async Task<IActionResult> Destroy(Guid id)
//        {
//            var command = new SoftDeleteCourseCommand
//            {
//                Id = id,
//                CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//            };
//            await _mediator.Send(command);
//            return Ok(new { message = "دوره حذف شد." });
//        }

//        // بازیابی (Restore)
//        [HttpPost("{id}/restore")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Restore(Guid id)
//        {
//            await _mediator.Send(new RestoreCourseCommand { Id = id });
//            return Ok(new { message = "دوره بازیابی شد." });
//        }

//        // حذف دائم (Force Delete)
//        [HttpDelete("{id}/force")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> ForceDelete(Guid id)
//        {
//            await _mediator.Send(new ForceDeleteCourseCommand { Id = id });
//            return Ok(new { message = "دوره به طور کامل حذف شد." });
//        }
//    }
//}
