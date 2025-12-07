using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Users;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;
using static Pardis.Query.Users.GetUsers.CreateUserByAdminHandler;

namespace Pardis.Application.Users.Commands // یا فضای نام درست پروژه شما
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, OperationResult<UserResource>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager; // اضافه شد برای چک کردن نقش‌ها
        private readonly IMapper _mapper;

        public UpdateUserHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<OperationResult<UserResource>> Handle(UpdateUserCommand request, CancellationToken token)
        {
            // 1. پیدا کردن کاربر
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return OperationResult<UserResource>.NotFound("کاربر یافت نشد.");

            // 2. آپدیت اطلاعات پایه
            user.FullName = request.FullName;
            user.Mobile = request.Mobile;

            // 3. تغییر ایمیل (توسط ادمین)
            if (user.Email != request.Email)
            {
                // چک کنیم ایمیل تکراری نباشد
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                    return OperationResult<UserResource>.Error("این ایمیل قبلاً توسط کاربر دیگری ثبت شده است.");

                user.Email = request.Email;
                user.UserName = request.Email;
                user.EmailConfirmed = true; // چون ادمین تغییر داده، تایید شده فرض می‌شود
            }

            // 4. ذخیره تغییرات پایه در دیتابیس
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return OperationResult<UserResource>.Error(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // 5. مدیریت نقش‌ها (بخش مهم برای ادمین)
            // اگر لیستی از نقش‌ها ارسال شده باشد (حتی لیست خالی برای حذف همه نقش‌ها)
            if (request.Roles != null)
            {
                // الف) دریافت نقش‌های فعلی کاربر
                var currentRoles = await _userManager.GetRolesAsync(user);

                // ب) حذف تمام نقش‌های فعلی
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return OperationResult<UserResource>.Error("خطا در حذف نقش‌های قبلی.");

                // ج) افزودن نقش‌های جدید (فقط آنهایی که در سیستم وجود دارند)
                if (request.Roles.Any())
                {
                    // فیلتر کردن نقش‌های نامعتبر احتمالی
                    var validRoles = new List<string>();
                    foreach (var role in request.Roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            validRoles.Add(role);
                        }
                    }

                    if (validRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, validRoles);
                    }
                }
            }

            // 6. آماده‌سازی خروجی
            var resource = _mapper.Map<UserResource>(user);
            resource.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return OperationResult<UserResource>.Success(resource);
        }
    }
}