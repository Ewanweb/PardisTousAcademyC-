namespace Pardis.Domain.Dto.Users;

public class UserResource
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required string Mobile { get; set; }
    public string? IsActive { get; set; }
    public string? Email { get; set; } // ایمیل اختیاری شد
    public List<string>? Roles { get; set; }
    
    // ❌ حذف شد: Courses - این باعث circular reference می‌شود
    // اگر نیاز به دوره‌های کاربر داری، یک endpoint جداگانه بساز
}
