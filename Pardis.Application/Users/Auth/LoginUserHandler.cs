using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.Auth;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, OperationResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<OperationResult> Handle(LoginUserCommand request, CancellationToken token)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return OperationResult.Error("کاربری با این مشخصات یافت نشد"); // در کنترلر Unauthorized برمی‌گردانیم

        if (!user.IsActive)
            return OperationResult.Error("حساب کاربری غیرفعال است.");

        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = _tokenService.GenerateToken(user, roles);

        return OperationResult.Success(jwtToken);
    }
}