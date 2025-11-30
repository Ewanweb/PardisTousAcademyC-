using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Users.Auth;
using Pardis.Query.Users.GetUserById;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Status != OperationResultStatus.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return CreatedAtAction(nameof(Login), new { message = "ثبت‌نام با موفقیت انجام شد.", data = result.Data });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            // لاگین و تولید توکن در هندلر انجام می‌شود
            var result = await _mediator.Send(command);

            if (result.Status != OperationResultStatus.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(new
            {
                message = "ورود موفقیت‌آمیز بود.",
                data = result.Data
            });
        }
        // ... متدهای Login و Register قبلی سر جای خود بمانند ...

        // --- این متد جدید را اضافه کنید ---
        [HttpGet("~/api/user")] // آدرس دقیق را با ~ مشخص می‌کنیم تا از پیشوند Auth رد شود
        [Authorize] // فقط کسانی که توکن دارند می‌توانند صدا بزنند
        public async Task<IActionResult> GetCurrentUser()
        {
            // 1. پیدا کردن آیدی کاربر از داخل توکن
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "توکن نامعتبر است." });

            // 2. درخواست اطلاعات کاربر از دیتابیس (استفاده از همان کوئری GetUserById که قبلا داشتید یا مشابه آن)
            // اگر کوئری GetUserByIdQuery ندارید، پایین توضیح دادم چکار کنید
            var result = await _mediator.Send(new GetUserByIdQuery { Id = userId });

            if (result == null)
                return NotFound(new { message = "کاربر یافت نشد." });

            // 3. ارسال اطلاعات به فرمتی که فرانت می‌خواهد
            return Ok(new
            {
                data = result // فرانت انتظار دارد اطلاعات داخل data باشد
            });
        }
    }
}

