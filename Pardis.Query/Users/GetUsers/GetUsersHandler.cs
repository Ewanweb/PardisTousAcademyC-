using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Users;
using Pardis.Domain.Users;

namespace Pardis.Query.Users.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserResource>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetUsersHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserResource>> Handle(GetUsersQuery request, CancellationToken token)
        {
            IQueryable<User> query;

            if (!string.IsNullOrEmpty(request.Role))
            {
                // دریافت کاربران با نقش خاص
                var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);
                query = usersInRole.AsQueryable();
            }
            else
            {
                // دریافت همه کاربران
                query = _userManager.Users;
            }

            // محاسبه تعداد کل
            var totalCount = await query.CountAsync(token);

            // اگر GetAll فعال نباشد، صفحه‌بندی اعمال می‌شود
            List<User> users;
            if (request.GetAll)
            {
                users = await query.ToListAsync(token);
            }
            else
            {
                var pagination = PaginationHelper.Normalize(new PaginationRequest
                {
                    Page = request.Page,
                    PageSize = request.PageSize
                });

                users = await query
                    .OrderBy(u => u.FullName)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(token);
            }

            // تبدیل به DTO
            var userResources = _mapper.Map<List<UserResource>>(users);

            // پر کردن لیست نقش‌ها برای هر کاربر
            foreach (var resource in userResources)
            {
                var user = users.First(u => u.Id == resource.Id);
                var roles = await _userManager.GetRolesAsync(user);
                resource.Roles = roles.ToList();
            }

            // ایجاد نتیجه صفحه‌بندی شده
            if (request.GetAll)
            {
                return new PagedResult<UserResource>
                {
                    Items = userResources,
                    Page = 1,
                    PageSize = totalCount,
                    TotalCount = totalCount,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrev = false
                };
            }

            var normalizedPagination = PaginationHelper.Normalize(new PaginationRequest
            {
                Page = request.Page,
                PageSize = request.PageSize
            });

            return PaginationHelper.Create(userResources, normalizedPagination, totalCount);
        }
    }
}
