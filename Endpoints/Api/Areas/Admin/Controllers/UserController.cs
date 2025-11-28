//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace Api.Areas.Admin.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(Roles = "Admin,Manager")] // پیش‌فرض برای کل کنترلر

//    public class UserController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public UserController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        // لیست کاربران با فیلتر نقش
//        [HttpGet]
//        public async Task<IActionResult> Index([FromQuery] string? role, [FromQuery] bool all = false)
//        {
//            var query = new GetUsersQuery { Role = role, GetAll = all };
//            var result = await _mediator.Send(query);
//            return Ok(result); // ریسورس کالکشن
//        }

//        // ایجاد کاربر توسط ادمین
//        [HttpPost]
//        public async Task<IActionResult> Store([FromBody] CreateUserByAdminCommand command)
//        {
//            var result = await _mediator.Send(command);
//            return StatusCode(201, new { message = "کاربر جدید ایجاد شد.", data = result });
//        }

//        // ویرایش کاربر
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
//        {
//            command.Id = id;
//            var result = await _mediator.Send(command);
//            return Ok(new { message = "اطلاعات کاربر ویرایش شد.", data = result });
//        }

//        // حذف کاربر
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Destroy(string id)
//        {
//            try
//            {
//                await _mediator.Send(new DeleteUserCommand { Id = id });
//                return Ok(new { message = "کاربر با موفقیت حذف شد." });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//        }

//        // تغییر نقش‌ها
//        [HttpPut("{id}/roles")]
//        [Authorize(Roles = "Admin")] // فقط ادمین اصلی
//        public async Task<IActionResult> UpdateRoles(string id, [FromBody] List<string> roles)
//        {
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            // جلوگیری از تغییر نقش خودِ مدیر
//            if (id == currentUserId)
//                return StatusCode(403, new { message = "نمی‌توانید نقش خودتان را تغییر دهید." });

//            var result = await _mediator.Send(new UpdateUserRolesCommand { UserId = id, Roles = roles });
//            return Ok(new { message = "نقش‌های کاربر بروزرسانی شد.", data = result });
//        }
//    }
//}


