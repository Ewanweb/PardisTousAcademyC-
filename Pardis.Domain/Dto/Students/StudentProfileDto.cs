namespace Pardis.Domain.Dto.Students;

public class StudentProfileDto
{
    public string StudentId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime RegisterDate { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public long TotalAmountOwed { get; set; }
    public long TotalAmountPaid { get; set; }
    public long TotalRemainingAmount { get; set; }
    public List<StudentEnrollmentSummaryDto> Enrollments { get; set; } = new();
}