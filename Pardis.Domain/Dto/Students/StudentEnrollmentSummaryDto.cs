namespace Pardis.Domain.Dto.Students;

public class StudentEnrollmentSummaryDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public long TotalAmount { get; set; }
    public long PaidAmount { get; set; }
    public long RemainingAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentStatusDisplay { get; set; } = string.Empty;
    public string EnrollmentStatus { get; set; } = string.Empty;
    public string EnrollmentStatusDisplay { get; set; } = string.Empty;
    public decimal PaymentPercentage { get; set; }
    public bool IsInstallmentAllowed { get; set; }
    public int? InstallmentCount { get; set; }
}