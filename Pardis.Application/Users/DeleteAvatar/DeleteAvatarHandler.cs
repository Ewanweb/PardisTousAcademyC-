using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.DeleteAvatar;

public class DeleteAvatarHandler : IRequestHandler<DeleteAvatarCommand, OperationResult<UserProfileDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private const string AvatarDirectory = "uploads/avatars";

    public DeleteAvatarHandler(UserManager<User> userManager, IFileService fileService, IMapper mapper)
    {
        _userManager = userManager;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserProfileDto>> Handle(DeleteAvatarCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return OperationResult<UserProfileDto>.Error("شناسه کاربر یافت نشد");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return OperationResult<UserProfileDto>.NotFound("کاربر یافت نشد");

        if (string.IsNullOrEmpty(user.AvatarFileId))
            return OperationResult<UserProfileDto>.Error("آواتاری برای حذف وجود ندارد");

        try
        {
            // Delete avatar file
            _fileService.DeleteFile(AvatarDirectory, user.AvatarFileId);

            // Clear avatar info from user
            user.AvatarFileId = null;
            user.AvatarUrl = null;
            user.AvatarUpdatedAt = DateTime.UtcNow;
            user.Avatar = null; // Backward compatibility

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return OperationResult<UserProfileDto>.Error($"خطا در حذف آواتار: {errors}");
            }

            var profileDto = _mapper.Map<UserProfileDto>(user);
            return OperationResult<UserProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            return OperationResult<UserProfileDto>.Error($"خطا در حذف فایل آواتار: {ex.Message}");
        }
    }
}