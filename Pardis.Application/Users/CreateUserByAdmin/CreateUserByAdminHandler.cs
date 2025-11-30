using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsers;

public partial class CreateUserByAdminHandler : IRequestHandler<CreateUserByAdminCommand, OperationResult<UserResource>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public CreateUserByAdminHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<OperationResult<UserResource>> Handle(CreateUserByAdminCommand request, CancellationToken token)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Mobile = request.Mobile,
            IsActive = true,
            EmailConfirmed = true // چون توسط ادمین ساخته شده
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return OperationResult<UserResource>.Error(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // تخصیص نقش‌ها
        if (request.Roles != null && request.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, request.Roles);
        }
        else
        {
            // نقش پیش‌فرض
            await _userManager.AddToRoleAsync(user, Role.User);
        }

        var resource = _mapper.Map<UserResource>(user);
        resource.Roles = (await _userManager.GetRolesAsync(user)).ToList();

        return OperationResult<UserResource>.Success(resource);
    }
}