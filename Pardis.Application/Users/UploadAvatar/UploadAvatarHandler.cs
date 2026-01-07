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
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    // Allowed image types and max size
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private const long MaxFileSize = 2 * 1024 * 1024; // 2MB
    private const string AvatarDirectory = "uploads/avatars";

    public UploadAvatarHandler(UserManager<User> userManager, IFileService fileService, IMapper mapper)
    {
        _userManager = userManager;
        _fileService = fileService;
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
            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(user.AvatarFileId))
            {
                try
                {
                    _fileService.DeleteFile(AvatarDirectory, user.AvatarFileId);
                }
                catch
                {
                    // Log but don't fail if old file deletion fails
                }
            }

            // Save new avatar
            var fileName = await _fileService.SaveFileAndGenerateName(request.Avatar, AvatarDirectory);
            
            // Update user avatar info
            user.AvatarFileId = fileName;
            user.AvatarUrl = $"/{AvatarDirectory}/{fileName}";
            user.AvatarUpdatedAt = DateTime.UtcNow;
            
            // Keep backward compatibility with existing Avatar field
            user.Avatar = user.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Clean up uploaded file if user update fails
                try
                {
                    _fileService.DeleteFile(AvatarDirectory, fileName);
                }
                catch { }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return OperationResult<UserProfileDto>.Error($"خطا در به‌روزرسانی آواتار: {errors}");
            }

            var profileDto = _mapper.Map<UserProfileDto>(user);

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
