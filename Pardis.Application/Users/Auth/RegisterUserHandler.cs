using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Users;
using System.Data;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Users.Auth;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OperationResult<AuthResultDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RegisterUserHandler(UserManager<User> userManager, ITokenService tokenService, RoleManager<Role> roleManager, IUserRepository userRepository, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<OperationResult<AuthResultDto>> Handle(RegisterUserCommand request, CancellationToken token)
    {
        if ( await _userRepository.EmailIsExist(request.Email))
            return OperationResult<AuthResultDto>.Error("این ایمیل قبلا ثبت شده است");

        if (await _userRepository.MobileIsExist(request.Mobile))
            return OperationResult<AuthResultDto>.Error("این موبایل قبلا ثبت شده است");

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
            return OperationResult<AuthResultDto>.Error("خطا غیر منتظره رخ داد");

        // تخصیص نقش پیش‌فرض (Student)
        await _userManager.AddToRoleAsync(user, Role.User);

        var roles = new List<string> { Role.User };
        var jwtToken = _tokenService.GenerateToken(user, new List<string> { Role.User });

        var userResource = _mapper.Map<UserResource>(user);
        userResource.Roles = roles;

        var resultData = new AuthResultDto
        {
            Token = jwtToken,
            User = userResource
        };

        return OperationResult<AuthResultDto>.Success(resultData);
    }
}