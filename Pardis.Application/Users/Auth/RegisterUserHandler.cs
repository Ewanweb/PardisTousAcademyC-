using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.Auth;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OperationResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(UserManager<User> userManager, ITokenService tokenService, RoleManager<Role> roleManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
        _userRepository = userRepository;
    }
    public async Task<OperationResult> Handle(RegisterUserCommand request, CancellationToken token)
    {
        if ( await _userRepository.EmailIsExist(request.Email))
            return OperationResult.Error("این ایمیل قبلا ثبت شده است");

        if (await _userRepository.MobileIsExist(request.Mobile))
            return OperationResult.Error("این موبایل قبلا ثبت شده است");

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Mobile = request.Mobile,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return OperationResult.Error("خطا غیر منتظره رخ داد");

        // تخصیص نقش پیش‌فرض (Student)
        await _userManager.AddToRoleAsync(user, Role.User);

        var jwtToken = _tokenService.GenerateToken(user, new List<string> { Role.User });

        return OperationResult.Success(jwtToken);
    }
}