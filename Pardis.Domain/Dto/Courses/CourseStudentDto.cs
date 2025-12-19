namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO برای نمایش اطلاعات دانشجوی یک دوره
/// </summary>
public class CourseStudentDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string EnrollmentStatus { get; set; } = string.Empty;
    public string EnrollmentStatusDisplay { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentStatusDisplay { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public long PaidAmount { get; set; }
    public long RemainingAmount { get; set; }
    public decimal PaymentPercentage { get; set; }
    
    // اطلاعات حضور و غیاب
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public int AbsentSessions { get; set; }
    public double AttendancePercentage { get; set; }
    
    // Alias برای سازگاری با فرانت‌اند
    public double AttendanceRate => AttendancePercentage;
}