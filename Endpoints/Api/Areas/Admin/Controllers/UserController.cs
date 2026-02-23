using Api.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Pardis.Application._Shared;
using Pardis.Application.Users.UpdateUserRole;
using Pardis.Query.Users.GetRoles;
using Pardis.Query.Users.GetUsers;
using Pardis.Query.Users.GetUsersByRole;
using Pardis.Query.Users.GetUserById;
using System.Security.Claims;
using static Pardis.Query.Users.GetUsers.CreateUserByAdminHandler;
using Api.Controllers;
using Pardis.Application.Users.CreateUserByAdmin;

namespace Pardis.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت کاربران - فقط برای ادمین و منیجر
    /// </summary>
    [Route("api/[controller]s")]
    [ApiController]
    [Authorize(Policy = Policies.UserManagement.Access)]
    [Produces("application/json")]
    [Tags("Users Management")]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// سازنده کنترلر مدیریت کاربران
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public UserController(IMediator mediator, ILogger<UserController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست کاربران با قابلیت فیلتر و صفحه‌بندی
        /// </summary>
        /// <param name="role">نقش کاربر برای فیلتر</param>
        /// <param name="all">دریافت همه کاربران بدون صفحه‌بندی</param>
        /// <param name="page">شماره صفحه (پیش‌فرض: 1)</param>
        /// <param name="pageSize">تعداد آیتم در هر صفحه (پیش‌فرض: 20)</param>
        /// <returns>لیست صفحه‌بندی شده کاربران</returns>
        /// <response code="200">لیست کاربران با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی - فقط ادمین و منیجر</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Index(
            [FromQuery] string? role, 
            [FromQuery] bool all = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            return await ExecuteAsync(async () =>
            {
                var query = new GetUsersQuery 
                { 
                    Role = role?.Trim(), 
                    GetAll = all,
                    Page = page,
                    PageSize = pageSize
                };
                var result = await _mediator.Send(query);
                
                if (result == null || result.Items == null || !result.Items.Any())
                    return SuccessResponse(new { items = new List<object>(), page = 1, pageSize = 20, totalCount = 0, totalPages = 0 }, "هیچ کاربری یافت نشد");

                var message = string.IsNullOrWhiteSpace(role) 
                    ? $"{result.TotalCount} کاربر یافت شد"
                    : $"{result.TotalCount} کاربر با نقش {role} یافت شد";
                    
                return SuccessResponse(result, message);
            }, "خطا در دریافت لیست کاربران");
        }

        /// <summary>
        /// دریافت جزئیات کامل یک کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns>اطلاعات کامل کاربر</returns>
        /// <response code="200">اطلاعات کاربر با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="404">کاربر یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Show(string id)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return ValidationErrorResponse("شناسه کاربر الزامی است", new { id = "شناسه کاربر نمی‌تواند خالی باشد" });

                var query = new GetUserByIdQuery { Id = id.Trim() };
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFoundResponse("کاربر یافت نشد");

                return SuccessResponse(result, "اطلاعات کاربر با موفقیت دریافت شد");
            }, "خطا در دریافت اطلاعات کاربر");
        }

        /// <summary>
        /// دانلود آواتار کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <param name="env">محیط میزبانی وب</param>
        /// <returns>فایل آواتار</returns>
        /// <response code="200">فایل آواتار</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="404">کاربر یا آواتار یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id}/avatar/download")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DownloadAvatar(string id, [FromServices] IWebHostEnvironment env)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest(new { success = false, message = "شناسه کاربر الزامی است" });

                var query = new GetUserByIdQuery { Id = id.Trim() };
                var user = await _mediator.Send(query);

                if (user == null)
                    return NotFound(new { success = false, message = "کاربر یافت نشد" });

                if (string.IsNullOrWhiteSpace(user.AvatarFileId) && 
                    string.IsNullOrWhiteSpace(user.Avatar) && 
                    string.IsNullOrWhiteSpace(user.AvatarUrl))
                    return NotFound(new { success = false, message = "این کاربر آواتار ندارد" });

                // تلاش برای یافتن فایل از مسیرهای مختلف
                string? filePath = null;
                string? fileName = null;

                // اولویت 1: AvatarFileId
                if (!string.IsNullOrWhiteSpace(user.AvatarFileId))
                {
                    fileName = user.AvatarFileId;
                    if (fileName.Contains("/") || fileName.Contains("\\"))
                        fileName = Path.GetFileName(fileName);
                    
                    filePath = Path.Combine(env.WebRootPath, "uploads", "avatars", fileName);
                }

                // اولویت 2: Avatar
                if ((filePath == null || !System.IO.File.Exists(filePath)) && !string.IsNullOrWhiteSpace(user.Avatar))
                {
                    fileName = user.Avatar;
                    if (fileName.Contains("/") || fileName.Contains("\\"))
                        fileName = Path.GetFileName(fileName);
                    
                    filePath = Path.Combine(env.WebRootPath, "uploads", "avatars", fileName);
                }

                // اولویت 3: AvatarUrl - استخراج مسیر از URL
                if ((filePath == null || !System.IO.File.Exists(filePath)) && !string.IsNullOrWhiteSpace(user.AvatarUrl))
                {
                    // مثال: /uploads/avatars/filename.jpg
                    var urlPath = user.AvatarUrl.TrimStart('/');
                    filePath = Path.Combine(env.WebRootPath, urlPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    fileName = Path.GetFileName(filePath);
                }

                // بررسی وجود فایل
                if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("Avatar file not found for user {UserId}. Tried path: {FilePath}", id, filePath);
                    return NotFound(new { 
                        success = false, 
                        message = "فایل آواتار یافت نشد",
                        debug = new {
                            avatarFileId = user.AvatarFileId,
                            avatar = user.Avatar,
                            avatarUrl = user.AvatarUrl,
                            attemptedPath = filePath,
                            webRootPath = env.WebRootPath
                        }
                    });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
                
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                var downloadFileName = $"avatar-{user.FullName?.Replace(" ", "-") ?? "user"}{fileExtension}";
                
                return File(fileBytes, contentType, downloadFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دانلود آواتار کاربر {UserId}", id);
                return StatusCode(500, new { success = false, message = "خطا در دانلود آواتار", error = ex.Message });
            }
        }

        /// <summary>
        /// ایجاد کاربر جدید توسط ادمین یا منیجر
        /// </summary>
        /// <param name="command">اطلاعات کاربر جدید</param>
        /// <returns>اطلاعات کاربر ایجاد شده</returns>
        /// <response code="201">کاربر با موفقیت ایجاد شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="500">خطای سرور</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Store([FromBody] CreateUserByAdminCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                if (command == null)
                    return ValidationErrorResponse("اطلاعات کاربر الزامی است");

                if (string.IsNullOrWhiteSpace(command.Email))
                    return ValidationErrorResponse("ایمیل الزامی است", new { email = "ایمیل نمی‌تواند خالی باشد" });

                if (string.IsNullOrWhiteSpace(command.Password))
                    return ValidationErrorResponse("رمز عبور الزامی است", new { password = "رمز عبور نمی‌تواند خالی باشد" });

                if (string.IsNullOrWhiteSpace(command.FullName))
                    return ValidationErrorResponse("نام کامل الزامی است", new { fullName = "نام کامل نمی‌تواند خالی باشد" });

                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.Error)
                    return ErrorResponse(result.Message ?? "خطا در ایجاد کاربر", 400, "CREATE_USER_FAILED");

                return StatusCode(201, new { 
                    success = true,
                    message = "کاربر جدید با موفقیت ایجاد شد", 
                    data = result.Data,
                    timestamp = DateTime.UtcNow
                });
            }, "خطا در ایجاد کاربر");
        }

        /// <summary>
        /// بروزرسانی اطلاعات کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <param name="command">اطلاعات جدید کاربر</param>
        /// <returns>اطلاعات بروزرسانی شده کاربر</returns>
        /// <response code="200">کاربر با موفقیت بروزرسانی شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="404">کاربر یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return ValidationErrorResponse("شناسه کاربر الزامی است", new { id = "شناسه کاربر نمی‌تواند خالی باشد" });

                if (command == null)
                    return ValidationErrorResponse("اطلاعات کاربر الزامی است");

                if (string.IsNullOrWhiteSpace(command.FullName))
                    return ValidationErrorResponse("نام کامل الزامی است", new { fullName = "نام کامل نمی‌تواند خالی باشد" });

                command.Id = id.Trim();
                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.NotFound) 
                    return NotFoundResponse("کاربر یافت نشد");
                
                if (result.Status == OperationResultStatus.Error) 
                    return ErrorResponse(result.Message ?? "خطا در بروزرسانی کاربر", 400, "UPDATE_USER_FAILED");

                return SuccessResponse(result.Data, "اطلاعات کاربر با موفقیت بروزرسانی شد");
            }, "خطا در بروزرسانی کاربر");
        }

        /// <summary>
        /// حذف کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns>نتیجه حذف</returns>
        /// <response code="200">کاربر با موفقیت حذف شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی یا کاربر قابل حذف نیست</response>
        /// <response code="404">کاربر یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Destroy(string id)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return ValidationErrorResponse("شناسه کاربر الزامی است", new { id = "شناسه کاربر نمی‌تواند خالی باشد" });

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (id.Trim() == currentUserId)
                    return ErrorResponse("شما نمی‌توانید خودتان را حذف کنید", 403, "CANNOT_DELETE_SELF");

                try
                {
                    var result = await _mediator.Send(new DeleteUserCommand { Id = id.Trim() });
                    return SuccessResponse(new { id }, "کاربر با موفقیت حذف شد");
                }
                catch (UnauthorizedAccessException ex)
                {
                    return ErrorResponse(ex.Message, 403, "DELETE_FORBIDDEN");
                }
                catch (ArgumentException ex)
                {
                    return NotFoundResponse(ex.Message);
                }
            }, "خطا در حذف کاربر");
        }

        /// <summary>
        /// دریافت لیست نقش‌های موجود در سیستم
        /// </summary>
        /// <returns>لیست نقش‌ها</returns>
        /// <response code="200">لیست نقش‌ها با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("GetRoles")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetList()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetRolesQuery());
                
                if (result == null)
                    return SuccessResponse(new List<object>(), "هیچ نقشی تعریف نشده است");

                var roles = result as IEnumerable<object>;
                if (roles != null && roles.Any())
                {
                    return SuccessResponse(result, $"{roles.Count()} نقش در سیستم موجود است");
                }

                return SuccessResponse(new List<object>(), "هیچ نقشی تعریف نشده است");
            }, "خطا در دریافت لیست نقش‌ها");
        }

        /// <summary>
        /// بروزرسانی نقش‌های کاربر - فقط برای ادمین
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <param name="roles">لیست نقش‌های جدید</param>
        /// <returns>نتیجه بروزرسانی نقش‌ها</returns>
        /// <response code="200">نقش‌های کاربر با موفقیت بروزرسانی شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی - فقط ادمین</response>
        /// <response code="404">کاربر یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpPut("{id}/roles")]
        [Authorize(Policy = Policies.UserManagement.UpdateRoles)]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdateRoles(string id, [FromBody] List<string> roles)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return ValidationErrorResponse("شناسه کاربر الزامی است", new { id = "شناسه کاربر نمی‌تواند خالی باشد" });

                if (roles == null || roles.Count == 0)
                    return ValidationErrorResponse("حداقل یک نقش باید انتخاب شود", new { roles = "لیست نقش‌ها نمی‌تواند خالی باشد" });

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // جلوگیری از تغییر نقش خودِ مدیر توسط خودش
                if (id.Trim() == currentUserId)
                {
                    return ErrorResponse("شما نمی‌توانید نقش خودتان را تغییر دهید", 403, "CANNOT_CHANGE_OWN_ROLE");
                }

                var command = new UpdateUserRolesCommand
                {
                    UserId = id.Trim(),
                    Roles = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).ToList()
                };

                var result = await _mediator.Send(command);

                if (result.Status == OperationResultStatus.NotFound)
                    return NotFoundResponse("کاربر یافت نشد");

                if (result.Status == OperationResultStatus.Error)
                    return ErrorResponse(result.Message ?? "خطا در بروزرسانی نقش‌ها", 400, "UPDATE_ROLES_FAILED");

                return SuccessResponse(result.Data, "نقش‌های کاربر با موفقیت بروزرسانی شد");
            }, "خطا در بروزرسانی نقش‌های کاربر");
        }

        /// <summary>
        /// دریافت کاربران بر اساس نقش
        /// </summary>
        /// <param name="role">نقش مورد نظر</param>
        /// <returns>لیست کاربران با نقش مشخص</returns>
        /// <response code="200">لیست کاربران با موفقیت دریافت شد</response>
        /// <response code="400">درخواست نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("role/{role}")] 
        [Authorize(Policy = Policies.UserManagement.Access)]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetByRole(string role)
        {
            return await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(role))
                    return ValidationErrorResponse("نقش الزامی است", new { role = "نقش نمی‌تواند خالی باشد" });

                var query = new GetUsersByRoleQuery
                {
                    Role = role.Trim(),
                    All = true,
                    Page = 1
                };

                var result = await _mediator.Send(query);
                
                if (result == null)
                    return SuccessResponse(new List<object>(), $"هیچ کاربری با نقش {role} یافت نشد");

                var users = result as IEnumerable<object>;
                if (users != null && users.Any())
                {
                    return SuccessResponse(result, $"{users.Count()} کاربر با نقش {role} یافت شد");
                }

                return SuccessResponse(new List<object>(), $"هیچ کاربری با نقش {role} یافت نشد");
            }, "خطا در دریافت کاربران بر اساس نقش");
        }


    }
}