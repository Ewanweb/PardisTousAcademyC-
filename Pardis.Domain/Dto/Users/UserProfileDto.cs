using Pardis.Domain.Users;

namespace Pardis.Domain.Dto.Users;

public class UserProfileDto
{
    public required string Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public string? NationalCode { get; set; }
    public string? FatherName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? AvatarUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
