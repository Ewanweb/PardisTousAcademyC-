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
            // ✅ بهینه‌سازی: استفاده از UserManager برای گرفتن کاربران یک نقش خاص
            var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);

            // مرتب‌سازی و صفحه‌بندی
            var query = usersInRole.AsQueryable().OrderByDescending(u => u.CreatedAt);

            if (!request.All)
            {
                users = query.Skip((request.Page - 1) * request.PageSize)
                             .Take(request.PageSize)
                             .ToList();
            }
            else
            {
                // ✅ برای صفحه اصلی فقط 4 تا instructor کافیه
                users = query.Take(request.Role == Role.Instructor ? 4 : 10).ToList();
            }
        }
        else
        {
            // 2. دریافت همه کاربران (بدون فیلتر نقش)
            var allUsers = await _userRepository.GetAllAsync();
            var query = allUsers.AsQueryable().OrderByDescending(u => u.CreatedAt);
            
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

        // 3. ✅ بهینه‌سازی: تبدیل به Resource بدون query اضافی برای roles
        var userResources = _mapper.Map<List<UserResource>>(users);

        // 4. ✅ بهینه‌سازی: برای instructor ها نیاز به roles نیست در صفحه اصلی
        if (request.Role == Role.Instructor)
        {
            // فقط نام و عکس کافیه برای صفحه اصلی
            foreach (var resource in userResources)
            {
                resource.Roles = new List<string> { Role.Instructor };
            }
        }
        else
        {
            // فقط برای سایر موارد roles رو بکش
            foreach (var resource in userResources)
            {
                var user = users.First(u => u.Id == resource.Id);
                var roles = await _userManager.GetRolesAsync(user);
                resource.Roles = roles.ToList();
            }
        }

        return userResources;
    }
}
