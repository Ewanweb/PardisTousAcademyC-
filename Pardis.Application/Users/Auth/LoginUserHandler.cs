using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Service;
using Pardis.Domain.Users;

namespace Pardis.Application.Users.Auth
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, OperationResult<AuthResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IRequestContext _requestContext;
        private readonly IAuthLogRepository _authLogRepository;
    
        public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService, IMapper mapper, IAuthLogRepository authLogRepository, IRequestContext requestContext)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _authLogRepository = authLogRepository;
            _requestContext = requestContext;
        }

        public async Task<OperationResult<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken token)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Mobile);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return OperationResult<AuthResultDto>.Error("کاربری با این مشخصات یافت نشد");
                }

                if (!user.IsActive)
                {
                    return OperationResult<AuthResultDto>.Error("حساب کاربری غیرفعال است.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var jwtToken = _tokenService.GenerateToken(user, roles);



                var log = new AuthLog(_requestContext.GetPlatform()!, _requestContext.Ip(), user.Id);

                user.AuthLogs.Add(log);

                await _authLogRepository.AddAsync(log);

                var save = await _authLogRepository.SaveChangesAsync(cancellation: token);

                if (save <= 0)
                {
                    return OperationResult<AuthResultDto>.Error("خطا در ثبت اطلاعات ورود");
                }

                var authLogs = await _authLogRepository.GetUserAuthLogs(user.Id);

                var userResource = _mapper.Map<UserResource>(user);
                var authLogsMapped = _mapper.Map<List<AuthLogDTO>>(authLogs);
                userResource.Roles = roles.ToList();
                userResource.AuthLogs = authLogsMapped;

                // ساخت آبجکت نهایی
                var result = new AuthResultDto
                {
                    Token = jwtToken,
                    User = userResource
                };

                return OperationResult<AuthResultDto>.Success(result);

            }
            catch (Exception e)
            {
                return OperationResult<AuthResultDto>.Error("خطا غیر منتظره");
            }


            
        }
    }
}