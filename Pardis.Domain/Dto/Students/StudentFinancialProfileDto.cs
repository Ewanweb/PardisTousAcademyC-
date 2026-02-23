namespace Pardis.Domain.Dto.Students;

/// <summary>
/// DTO پروفایل مالی دانشجو
/// </summary>
public class StudentFinancialProfileDto
{
    public StudentBasicInfoDto Student { get; set; } = new();
    public List<EnrollmentFinancialDto> Enrollments { get; set; } = new();
}

/// <summary>
/// اطلاعات پایه دانشجو
/// </summary>
public class StudentBasicInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Avatar { get; set; }
}

/// <summary>
/// اطلاعات مالی ثبت‌نام
/// </summary>
public class EnrollmentFinancialDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public CourseBasicInfoDto Course { get; set; } = new();
    public long TotalAmount { get; set; }
    public long PaidAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string EnrollmentStatus { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public bool IsInstallmentAllowed { get; set; }
    public List<InstallmentDto> Installments { get; set; } = new();
}

/// <summary>
/// اطلاعات پایه دوره
/// </summary>
public class CourseBasicInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
}

/// <summary>
/// اطلاعات قسط
/// </summary>
public class InstallmentDto
{
    public Guid Id { get; set; }
    public int InstallmentNumber { get; set; }
    public long Amount { get; set; }
    public long PaidAmount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidDate { get; set; }
}
