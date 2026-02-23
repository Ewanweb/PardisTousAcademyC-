using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.UploadAvatar;

public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, OperationResult<UserProfileDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly ISecureFileService _secureFileService;
    private readonly IMapper _mapper;

    // Allowed image types and max size
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

    public UploadAvatarHandler(UserManager<User> userManager, ISecureFileService secureFileService, IMapper mapper)
    {
        _userManager = userManager;
        _secureFileService = secureFileService;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserProfileDto>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return OperationResult<UserProfileDto>.Error("شناسه کاربر یافت نشد");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OperationResult<UserProfileDto>.NotFound("کاربر یافت نشد");

        // Validate file
        var validationResult = ValidateAvatarFile(request.Avatar);
        if (!validationResult.IsValid)
            return OperationResult<UserProfileDto>.Error(validationResult.ErrorMessage);

        try
        {
            // Upload avatar using secure file service
            var uploadResult = await _secureFileService.SaveFileSecurely(
                request.Avatar,
                "avatars",
                user.Id
            );

            if (!uploadResult.IsSuccess)
                return OperationResult<UserProfileDto>.Error(uploadResult.ErrorMessage ?? "خطا در آپلود فایل");

            // Delete old avatar if exists (optional - old files can be kept for audit)
            // Note: We're not deleting old files to maintain audit trail
            
            // Update user avatar info
            // Store the file path for direct access (compatible with static file serving)
            user.AvatarFileId = uploadResult.SecureFileName;
            user.AvatarUrl = $"/uploads/{uploadResult.Category}/{uploadResult.SecureFileName}";
            user.AvatarUpdatedAt = DateTime.UtcNow;
            
            // Keep backward compatibility with existing Avatar field
            user.Avatar = user.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return OperationResult<UserProfileDto>.Error($"خطا در به‌روزرسانی آواتار: {errors}");
            }

            var profileDto = _mapper.Map<UserProfileDto>(user);

            // Add cache busting parameter
            if (!string.IsNullOrEmpty(profileDto.AvatarUrl))
            {
                profileDto.AvatarUrl = $"{profileDto.AvatarUrl}?v={user.AvatarUpdatedAt?.Ticks ?? DateTime.UtcNow.Ticks}";
            }

            return OperationResult<UserProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            return OperationResult<UserProfileDto>.Error($"خطا در آپلود فایل: {ex.Message}");
        }
    }

    private (bool IsValid, string ErrorMessage) ValidateAvatarFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "فایل انتخاب نشده است");

        if (file.Length > MaxFileSize)
            return (false, $"حجم فایل نباید بیش از {MaxFileSize / (1024 * 1024)} مگابایت باشد");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return (false, "فرمت فایل مجاز نیست. فرمت‌های مجاز: JPG, JPEG, PNG, WEBP");

        if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            return (false, "نوع فایل مجاز نیست");

        return (true, string.Empty);
    }
}
