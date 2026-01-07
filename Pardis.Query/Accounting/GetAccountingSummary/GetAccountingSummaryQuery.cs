using MediatR;

namespace Pardis.Query.Accounting.GetAccountingSummary;

/// <summary>
/// Query برای دریافت خلاصه آمار حسابداری
/// </summary>
public class GetAccountingSummaryQuery : IRequest<GetAccountingSummaryResult>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string Period { get; set; } = "month"; // today, week, month, year
}

/// <summary>
/// نتیجه خلاصه آمار حسابداری
/// </summary>
public class GetAccountingSummaryResult
{
    public long TotalRevenue { get; set; }
    public long MonthlyRevenue { get; set; }
    public long WeeklyRevenue { get; set; }
    public long TodayRevenue { get; set; }
    
    public int TotalTransactions { get; set; }
    public int MonthlyTransactions { get; set; }
    public int WeeklyTransactions { get; set; }
    public int TodayTransactions { get; set; }
    
    public int TotalActiveStudents { get; set; }
    public int MonthlyNewStudents { get; set; }
    public int WeeklyNewStudents { get; set; }
    public int TodayNewStudents { get; set; }
    
    public int PendingPayments { get; set; }
    public int ApprovedPayments { get; set; }
    public int RejectedPayments { get; set; }
    
    public long AverageOrderValue { get; set; }
    public decimal SuccessRate { get; set; }
    
    public List<RevenueByDayDto> RevenueChart { get; set; } = new();
    public List<PaymentMethodStatsDto> PaymentMethodStats { get; set; } = new();
}

public class RevenueByDayDto
{
    public DateTime Date { get; set; }
    public long Revenue { get; set; }
    public int TransactionCount { get; set; }
}

public class PaymentMethodStatsDto
{
    public string Method { get; set; } = string.Empty;
    public string MethodDisplay { get; set; } = string.Empty;
    public int Count { get; set; }
    public long TotalAmount { get; set; }
    public decimal Percentage { get; set; }
}