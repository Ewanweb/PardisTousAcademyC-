using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Users.Auth;
using Pardis.Query.Users.GetUserById;
using System.Security.Claims;
using Pardis.Domain.Users;
using Pardis.Query.Users.GetUserAuthLogs;

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
        private readonly IAuthLogRepository _authLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// سازنده کنترلر احراز هویت
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public AuthController(IMediator mediator, ILogger<AuthController> logger, IHttpContextAccessor httpContextAccessor, IAuthLogRepository repository) : base(logger)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _authLogRepository = repository;
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
                // اعتبارسنجی پارامترهای ورودی
                if (command == null)
                    return ValidationErrorResponse("اطلاعات ثبت‌نام الزامی است");

                if (string.IsNullOrWhiteSpace(command.Mobile))
                    return ValidationErrorResponse("شماره تلفن الزامی است", new { mobile = "شماره تلفن نمی‌تواند خالی باشد" });

                if (string.IsNullOrWhiteSpace(command.Password))
                    return ValidationErrorResponse("رمز عبور الزامی است", new { password = "رمز عبور نمی‌تواند خالی باشد" });

                if (string.IsNullOrWhiteSpace(command.FullName))
                    return ValidationErrorResponse("نام کامل الزامی است", new { fullName = "نام کامل نمی‌تواند خالی باشد" });

                var result = await _mediator.Send(command);
                
                if (result.Status == OperationResultStatus.Success)
                {
                    return StatusCode(201, new { 
                        success = true,
                        message = "ثبت‌نام با موفقیت انجام شد. اکنون می‌توانید وارد شوید", 
                        data = result.Data,
                        timestamp = DateTime.UtcNow
                    });
                }

                if (result.Status == OperationResultStatus.Error)
                {
                    return ErrorResponse(result.Message ?? "خطا در ثبت‌نام کاربر", 400, "REGISTRATION_FAILED");
                }

                return HandleOperationResult(result);
            }, "خطا در ثبت‌نام کاربر");
        }

        /// <summary>
        /// ورود کاربر به سیستم
        /// </summary>
        /// <param name="command">اطلاعات ورود شامل شماره تلفن و رمز عبور</param>
        /// <returns>اطلاعات کاربر و توکن احراز هویت</returns>
        /// <response code="200">ورود موفقیت‌آمیز</response>
        /// <response code="400">اطلاعات نامعتبر</response>
        /// <response code="401">شماره تلفن یا رمز عبور اشتباه</response>
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
                // اعتبارسنجی پارامترهای ورودی
                if (command == null)
                    return ValidationErrorResponse("اطلاعات ورود الزامی است");

                if (string.IsNullOrWhiteSpace(command.Mobile))
                    return ValidationErrorResponse("شماره تلفن الزامی است", new { mobile = "شماره تلفن نمی‌تواند خالی باشد" });

                if (string.IsNullOrWhiteSpace(command.Password))
                    return ValidationErrorResponse("رمز عبور الزامی است", new { password = "رمز عبور نمی‌تواند خالی باشد" });

                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.Success)
                    return SuccessResponse(result.Data, "ورود موفقیت‌آمیز بود. خوش آمدید!");
                

                // برای خطاهای احراز هویت، کد 401 برمی‌گردانیم
                if (result.Status == OperationResultStatus.Error)
                {
                    return UnauthorizedResponse(result.Message ?? "شماره تلفن یا رمز عبور اشتباه است");
                }

                return HandleOperationResult(result);
            }, "خطا در ورود به سیستم");
        }

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
                    return UnauthorizedResponse("توکن احراز هویت نامعتبر یا منقضی شده است");

                var result = await _mediator.Send(new GetUserByIdQuery { Id = userId });

                if (result == null)
                    return NotFoundResponse("کاربر یافت نشد. ممکن است حساب کاربری حذف شده باشد");

                return SuccessResponse(result, "اطلاعات کاربر با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات کاربر");
        }
        [HttpGet("authLog")]
        [Authorize]
        public async Task<IActionResult> GetUserAuthLog()
        {
            return await ExecuteAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return UnauthorizedResponse("توکن احراز هویت نامعتبر یا منقضی شده است");

                var query = new GetUserAuthLogsQuery(userId);

                var result = await _mediator.Send(query);

                if (result.Count == 0 || !result.Any())
                    return NotFoundResponse("کاربر یافت نشد. ممکن است حساب کاربری حذف شده باشد");

                return SuccessResponse(result, "اطلاعات کاربر با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات کاربر");
        }
    }
}

