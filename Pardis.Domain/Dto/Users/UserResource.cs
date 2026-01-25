using Pardis.Domain.Users;

namespace Pardis.Domain.Dto.Users;

public class UserResource
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required string Mobile { get; set; }
    public string? IsActive { get; set; }
    public string? Email { get; set; } // ایمیل اختیاری شد
    public string? AvatarUrl { get; set; }
    public DateTime? AvatarUpdatedAt { get; set; }
    public List<string>? Roles { get; set; }
    public List<AuthLogDTO>? AuthLogs { get; set; }
    
    // ❌ حذف شد: Courses - این باعث circular reference می‌شود
    // اگر نیاز به دوره‌های کاربر داری، یک endpoint جداگانه بساز
}
