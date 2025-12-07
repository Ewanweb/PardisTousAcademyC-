using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResource>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserResource> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null) return null;

            var resource = _mapper.Map<UserResource>(user);

            // پر کردن نقش‌ها
            var roles = await _userManager.GetRolesAsync(user);
            resource.Roles = roles.ToList();

            return resource;
        }
    }
}
