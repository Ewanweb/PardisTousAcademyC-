using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
//using Pardis.Application.Users.Commands;
//using Pardis.Application.Users.Queries;
using Pardis.Application.Users.UpdateUserRole;
using Pardis.Domain.Users;
using Pardis.Query.Users.GetRoles;
using Pardis.Query.Users.GetUsers;
using Pardis.Query.Users.GetUsersByRole;
using System.Security.Claims;
using static Pardis.Query.Users.GetUsers.CreateUserByAdminHandler;

namespace Pardis.API.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    // دسترسی پیش‌فرض برای مدیر و ادمین
    [Authorize(Roles = Role.Admin + "," + Role.Manager)]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // لیست کاربران با قابلیت فیلتر
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? role, [FromQuery] bool all = false)
        {
            var query = new GetUsersQuery { Role = role, GetAll = all };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // ایجاد کاربر جدید (توسط ادمین/منیجر)
        [HttpPost]
        public async Task<IActionResult> Store([FromBody] CreateUserByAdminCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.Error)
                return BadRequest(result.Message);

            return StatusCode(201, new { message = "کاربر جدید ایجاد شد.", data = result.Data });
        }

        // ویرایش اطلاعات کاربر
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);
            if (result.Status == OperationResultStatus.Error) return BadRequest(result.Message);

            return Ok(new { message = "اطلاعات کاربر ویرایش شد.", data = result.Data });
        }

        // حذف کاربر
        [HttpDelete("{id}")]
        public async Task<IActionResult> Destroy(string id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteUserCommand { Id = id });
                return Ok(new { message = "کاربر با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetList()
        {
            var result = await _mediator.Send(new GetRolesQuery());
            return Ok(new { data = result });
        }

        // تغییر نقش‌ها (مخصوص ادمین اصلی)
        [HttpPut("{id}/roles")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateRoles(string id, [FromBody] List<string> roles)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // جلوگیری از تغییر نقش خودِ مدیر توسط خودش
            if (id == currentUserId)
            {
                return BadRequest("شما نمی‌توانید نقش خودتان را تغییر دهید.");
            }

            var command = new UpdateUserRolesCommand
            {
                UserId = id,
                Roles = roles
            };

            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.NotFound)
                return NotFound(result.Message);

            if (result.Status == OperationResultStatus.Error)
                return BadRequest(result.Message);

            return Ok(new { message = "نقش‌های کاربر بروزرسانی شد.", data = result.Data });
        }

        [HttpGet("role/{role}")] 
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var query = new GetUsersByRoleQuery
            {
                Role = role,
                All = true,
                Page = 1
            };

            var result = await _mediator.Send(query);
            return Ok(new { data = result });
        }


    }
}