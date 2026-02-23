using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.UpdateProfile;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, OperationResult<UserProfileDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UpdateUserProfileHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Get current user from context (will be set by controller)
        var userId = request.UserId;
        if (string.IsNullOrEmpty(userId))
            return OperationResult<UserProfileDto>.Error("شناسه کاربر یافت نشد");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return OperationResult<UserProfileDto>.NotFound("کاربر یافت نشد");

        string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        // Update profile fields (only overwrite when provided)
        if (request.FullName != null)
        {
            user.FullName = Normalize(request.FullName);
        }

        if (request.Bio != null)
        {
            user.Bio = Normalize(request.Bio);
        }

        if (request.BirthDate.HasValue)
        {
            user.BirthDate = request.BirthDate;
        }

        if (request.Gender.HasValue)
        {
            user.Gender = request.Gender;
        }

        if (request.Address != null)
        {
            user.Address = Normalize(request.Address);
        }

        if (request.NationalCode != null)
        {
            user.NationalCode = Normalize(request.NationalCode);
        }

        if (request.FatherName != null)
        {
            user.FatherName = Normalize(request.FatherName);
        }

        if (request.PhoneNumber != null)
        {
            var normalizedPhone = Normalize(request.PhoneNumber);
            user.PhoneNumber = normalizedPhone;
            user.Mobile = normalizedPhone;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return OperationResult<UserProfileDto>.Error($"خطا در به‌روزرسانی پروفایل: {errors}");
        }

        var profileDto = _mapper.Map<UserProfileDto>(user);
        
        // ✅ اضافه کردن cache busting برای avatar
        if (!string.IsNullOrEmpty(profileDto.AvatarUrl))
        {
            profileDto.AvatarUrl = $"{profileDto.AvatarUrl}?v={user.AvatarUpdatedAt?.Ticks ?? DateTime.UtcNow.Ticks}";
        }

        return OperationResult<UserProfileDto>.Success(profileDto);
    }
}
