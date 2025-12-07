using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Domain;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsersByRole;

public class GetUsersByRoleHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<UserResource>>
{
    private readonly IRepository<User> _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public GetUsersByRoleHandler(IRepository<User> userRepository, UserManager<User> userManager, IMapper mapper)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResource>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        List<User> users;

        // 1. فیلتر بر اساس نقش
        if (!string.IsNullOrEmpty(request.Role))
        {
            // استفاده از UserManager برای گرفتن کاربران یک نقش خاص
            var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);

            // مرتب‌سازی و تبدیل به کوئری (در حافظه)
            var query = usersInRole.AsQueryable().OrderByDescending(u => u.CreatedAt);

            // صفحه‌بندی
            if (!request.All)
            {
                users = query.Skip((request.Page - 1) * request.PageSize)
                             .Take(request.PageSize)
                             .ToList();
            }
            else
            {
                users = query.ToList();
            }
        }
        else
        {
            // 2. دریافت همه کاربران (بدون فیلتر نقش) با استفاده از Repository
            // فرض بر این است که ریپازیتوری شما متدی مثل Table یا GetQuery برای دسترسی به IQueryable دارد
            // اگر متد خاصی دارید (مثل GetAllAsync)، آن را جایگزین کنید
            var allUsers = await _userRepository.GetAllAsync();

            // تبدیل به IQueryable برای اعمال فیلترها در حافظه
            var query = allUsers.AsQueryable().OrderByDescending(u => u.CreatedAt);
            if (!request.All)
            {
                // صفحه‌بندی روی دیتابیس انجام می‌شود
                users = query.Skip((request.Page - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .ToList();
            }
            else
            {
                users = query.ToList();
            }
        }

        // 3. تبدیل به Resource با استفاده از AutoMapper
        var userResources = _mapper.Map<List<UserResource>>(users);

        // 4. پر کردن نقش‌ها (چون معمولاً در مپینگ خودکار Identity پر نمی‌شوند)
        foreach (var resource in userResources)
        {
            var user = users.First(u => u.Id == resource.Id);
            var roles = await _userManager.GetRolesAsync(user);
            resource.Roles = roles.ToList();
        }

        return userResources;
    }
}
