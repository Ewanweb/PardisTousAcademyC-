using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserResource>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetUsersHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<UserResource>> Handle(GetUsersQuery request, CancellationToken token)
        {
            IList<User> users;

            if (!string.IsNullOrEmpty(request.Role))
            {
                // استفاده از متد داخلی Identity برای گرفتن کاربران یک نقش خاص
                users = await _userManager.GetUsersInRoleAsync(request.Role);
            }
            else
            {
                // دریافت همه کاربران
                users = await _userManager.Users.ToListAsync(token);
            }

            // تبدیل به DTO
            var userResources = _mapper.Map<List<UserResource>>(users);

            // پر کردن لیست نقش‌ها برای هر کاربر (چون Identity نقش‌ها را جدا نگه می‌دارد)
            // نکته: برای تعداد زیاد کاربر، این روش N+1 Query ایجاد می‌کند که می‌توان با کوئری مستقیم SQL بهینه کرد.
            // اما برای پنل ادمین با پیجینیشن معمولاً قابل قبول است.
            foreach (var resource in userResources)
            {
                var user = users.First(u => u.Id == resource.Id);
                var roles = await _userManager.GetRolesAsync(user);
                resource.Roles = roles.ToList();
            }

            return userResources;
        }
    }
}
