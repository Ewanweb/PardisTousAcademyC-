using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Users.UpdateUserRole
{
    public class UpdateUserRolesHandler : IRequestHandler<UpdateUserRolesCommand, OperationResult<UserResource>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public UpdateUserRolesHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<OperationResult<UserResource>> Handle(UpdateUserRolesCommand request, CancellationToken token)
        {
            // 1. پیدا کردن کاربر
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return OperationResult<UserResource>.Error("کاربر مورد نظر یافت نشد.");

            // 2. اعتبارسنجی نقش‌های ارسالی (اختیاری ولی توصیه شده)
            foreach (var role in request.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    return OperationResult<UserResource>.Error($"نقش '{role}' در سیستم وجود ندارد.");
            }

            // 3. دریافت نقش‌های فعلی کاربر
            var currentRoles = await _userManager.GetRolesAsync(user);

            // 4. حذف تمام نقش‌های فعلی
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return OperationResult<UserResource>.Error("خطا در حذف نقش‌های قبلی.");

            // 5. افزودن نقش‌های جدید
            var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
            if (!addResult.Succeeded)
                return OperationResult<UserResource>.Error("خطا در افزودن نقش‌های جدید.");

            // 6. آماده‌سازی خروجی
            var userResource = _mapper.Map<UserResource>(user);
            userResource.Roles = request.Roles; // نقش‌های جدید را در خروجی ست می‌کنیم

            return OperationResult<UserResource>.Success(userResource);
        }
    }
}
