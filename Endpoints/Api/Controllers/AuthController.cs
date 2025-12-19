using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Users.Auth;
using Pardis.Query.Users.GetUserById;
using System.Security.Claims;

namespace Api.Controllers
{
    /// <summary>
    /// کنترلر احراز هویت - ثبت‌نام، ورود و مدیریت کاربران
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Authentication")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator, ILogger<AuthController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// ثبت‌نام کاربر جدید در سیستم
        /// </summary>
        /// <param name="command">اطلاعات ثبت‌نام شامل ایمیل، رمز عبور، نام کامل و موبایل</param>
        /// <returns>اطلاعات کاربر و توکن احراز هویت</returns>
        /// <response code="201">ثبت‌نام با موفقیت انجام شد</response>
        /// <response code="400">اطلاعات نامعتبر یا ایمیل تکراری</response>
        /// <response code="500">خطای سرور</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                if (command == null)
                    return BadRequest(new { 
                        success = false, 
                        message = "اطلاعات ثبت‌نام الزامی است" 
                    });

                var result = await _mediator.Send(command);
                
                if (result.Status == OperationResultStatus.Success)
                {
                    return CreatedAtAction(nameof(Login), null, new { 
                        success = true,
                        message = "ثبت‌نام با موفقیت انجام شد", 
                        data = result.Data 
                    });
                }

                return HandleOperationResult(result);
            }, "خطا در ثبت‌نام کاربر");
        }

        /// <summary>
        /// ورود کاربر به سیستم
        /// </summary>
        /// <param name="command">اطلاعات ورود شامل ایمیل و رمز عبور</param>
        /// <returns>اطلاعات کاربر و توکن احراز هویت</returns>
        /// <response code="200">ورود موفقیت‌آمیز</response>
        /// <response code="400">اطلاعات نامعتبر</response>
        /// <response code="401">ایمیل یا رمز عبور اشتباه</response>
        /// <response code="500">خطای سرور</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                if (command == null)
                    return BadRequest(new { 
                        success = false, 
                        message = "اطلاعات ورود الزامی است" 
                    });

                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "ورود موفقیت‌آمیز بود",
                        data = result.Data
                    });
                }

                // برای خطاهای احراز هویت، کد 401 برمی‌گردانیم
                if (result.Status == OperationResultStatus.Error)
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = result.Message ?? "نام کاربری یا رمز عبور اشتباه است" 
                    });
                }

                return HandleOperationResult(result);
            }, "خطا در ورود به سیستم");
        }
        // ... متدهای Login و Register قبلی سر جای خود بمانند ...

        /// <summary>
        /// دریافت اطلاعات کاربر فعلی بر اساس توکن
        /// </summary>
        /// <returns>اطلاعات کاربر احراز هویت شده</returns>
        /// <response code="200">اطلاعات کاربر با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت یا توکن نامعتبر</response>
        /// <response code="404">کاربر یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("~/api/user")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { 
                        success = false, 
                        message = "توکن نامعتبر است" 
                    });

                var result = await _mediator.Send(new GetUserByIdQuery { Id = userId });

                if (result == null)
                    return NotFound(new { 
                        success = false, 
                        message = "کاربر یافت نشد" 
                    });

                return SuccessResponse(result, "اطلاعات کاربر با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات کاربر");
        }
    }
}

