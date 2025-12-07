using MediatR;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsersByRole;

public class GetUsersByRoleQuery : IRequest<IEnumerable<UserResource>>
{
    public string? Role { get; set; } // مثلا "Instructor"
    public bool All { get; set; } = false; // اگر true باشد، صفحه‌بندی غیرفعال می‌شود (برای لیست‌های کشویی)
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}
