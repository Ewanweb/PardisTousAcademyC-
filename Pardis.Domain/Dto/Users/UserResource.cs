namespace Pardis.Domain.Dto.Users;

public class UserResource
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public string? Mobile { get; set; }
    public string? IsActive { get; set; }
    public required string Email { get; set; }
    public List<string>? Roles { get; set; }
    
    // ❌ حذف شد: Courses - این باعث circular reference می‌شود
    // اگر نیاز به دوره‌های کاربر داری، یک endpoint جداگانه بساز
}
