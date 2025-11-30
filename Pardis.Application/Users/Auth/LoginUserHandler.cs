using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Dto; // اضافه شد
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos; // برای دسترسی به UserResource

namespace Pardis.Application.Users.Auth
{
    // تغییر مهم: خروجی به <AuthResultDto> تغییر کرد
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, OperationResult<AuthResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper; // اضافه شد

        public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return OperationResult<AuthResultDto>.Error("کاربری با این مشخصات یافت نشد");

            if (!user.IsActive)
                return OperationResult<AuthResultDto>.Error("حساب کاربری غیرفعال است.");

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.GenerateToken(user, roles);

            // تبدیل اطلاعات کاربر به فرمت مناسب فرانت
            var userResource = _mapper.Map<UserResource>(user);
            // دستی پر کردن نقش‌ها (چون اتومپر دسترسی به roleManager ندارد)
            userResource.Roles = roles.ToList();

            // ساخت آبجکت نهایی
            var result = new AuthResultDto
            {
                Token = jwtToken,
                User = userResource
            };

            return OperationResult<AuthResultDto>.Success(result);
        }
    }
}