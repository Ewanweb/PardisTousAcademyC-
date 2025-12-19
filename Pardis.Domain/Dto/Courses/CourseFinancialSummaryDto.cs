namespace Pardis.Domain.Dto.Courses;

/// <summary>
/// DTO برای خلاصه مالی یک دوره
/// </summary>
public class CourseFinancialSummaryDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long TotalRevenue { get; set; }
    public long PendingAmount { get; set; }
    public long OverdueAmount { get; set; }
    public int TotalStudents { get; set; }
    public int PaidStudents { get; set; }
    public int PartialPaidStudents { get; set; }
    public int UnpaidStudents { get; set; }
    public decimal AveragePaymentProgress { get; set; }
    public int OverdueInstallments { get; set; }
    public DateTime LastPaymentDate { get; set; }
}