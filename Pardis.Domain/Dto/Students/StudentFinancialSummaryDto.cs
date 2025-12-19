namespace Pardis.Domain.Dto.Students;

public class StudentFinancialSummaryDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
    public long TotalAmountOwed { get; set; }
    public long TotalAmountPaid { get; set; }
    public long TotalRemainingAmount { get; set; }
    public int PaidEnrollments { get; set; }
    public int PartialPaidEnrollments { get; set; }
    public int UnpaidEnrollments { get; set; }
    public int OverdueInstallments { get; set; }
    public decimal PaymentPercentage { get; set; }
}