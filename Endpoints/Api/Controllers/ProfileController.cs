using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Users.DeleteAvatar;
using Pardis.Application.Users.UpdateProfile;
using Pardis.Application.Users.UploadAvatar;
using Pardis.Query.Users.GetProfile;

namespace Api.Controllers;

/// <summary>
/// کنترلر مدیریت پروفایل کاربر
/// </summary>
[Route("api/my/profile")]
[Authorize]
public class ProfileController : BaseController
{
    public ProfileController(IMediator mediator, ILogger<ProfileController> logger) 
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// دریافت اطلاعات پروفایل کاربر فعلی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            var query = new GetUserProfileQuery { UserId = userId };
            var result = await Mediator.Send(query);

            return HandleOperationResult(result, "اطلاعات پروفایل با موفقیت دریافت شد");
        });
    }

    /// <summary>
    /// به‌روزرسانی اطلاعات پروفایل کاربر
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            // Set user ID from authenticated user (security measure)
            command.UserId = userId;

            var result = await Mediator.Send(command);
            return HandleOperationResult(result, "پروفایل با موفقیت به‌روزرسانی شد");
        });
    }

    /// <summary>
    /// آپلود آواتار کاربر
    /// </summary>
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile avatar)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            if (avatar == null || avatar.Length == 0)
                return ValidationErrorResponse("فایل آواتار انتخاب نشده است");

            var command = new UploadAvatarCommand 
            { 
                Avatar = avatar, 
                UserId = userId 
            };

            var result = await Mediator.Send(command);
            return HandleOperationResult(result, "آواتار با موفقیت آپلود شد");
        });
    }

    /// <summary>
    /// حذف آواتار کاربر
    /// </summary>
    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("کاربر احراز هویت نشده است");

            var command = new DeleteAvatarCommand { UserId = userId };
            var result = await Mediator.Send(command);

            return HandleOperationResult(result, "آواتار با موفقیت حذف شد");
        });
    }
}