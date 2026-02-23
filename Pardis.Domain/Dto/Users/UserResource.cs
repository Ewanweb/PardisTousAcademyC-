using Pardis.Domain.Users;

namespace Pardis.Domain.Dto.Users;

public class UserResource
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required string Mobile { get; set; }
    public string? IsActive { get; set; }
    public string? Email { get; set; } // ایمیل اختیاری شد
    public string? UserName { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Avatar { get; set; }
    public string? AvatarFileId { get; set; }
    public DateTime? AvatarUpdatedAt { get; set; }
    
    // Extended Profile Fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public string? NationalCode { get; set; }
    public string? FatherName { get; set; }
    
    public List<string>? Roles { get; set; }
    public List<AuthLogDTO>? AuthLogs { get; set; }
    
    // ❌ حذف شد: Courses - این باعث circular reference می‌شود
    // اگر نیاز به دوره‌های کاربر داری، یک endpoint جداگانه بساز
}
