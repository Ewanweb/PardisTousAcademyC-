using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.Auth
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, OperationResult<AuthResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken token)
        {
            // جستجو بر اساس شماره تلفن (که به عنوان UserName ذخیره شده)
            var user = await _userManager.FindByNameAsync(request.Mobile);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return OperationResult<AuthResultDto>.Error("کاربری با این مشخصات یافت نشد");

            if (!user.IsActive)
                return OperationResult<AuthResultDto>.Error("حساب کاربری غیرفعال است.");

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.GenerateToken(user, roles);

            // تبدیل اطلاعات کاربر به فرمت مناسب فرانت
            var userResource = _mapper.Map<UserResource>(user);
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