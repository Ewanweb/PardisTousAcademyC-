using Pardis.Domain.Accounting;

namespace Pardis.Domain.Dto.Accounting;

/// <summary>
/// DTO نمایش تراکنش
/// </summary>
public class TransactionDto
{
    public required Guid Id { get; set; }
    public required string TransactionId { get; set; }
    public required string StudentName { get; set; }
    public required string StudentEmail { get; set; }
    public required string CourseName { get; set; }
    public required long Amount { get; set; }
    public required string Status { get; set; }
    public required string StatusDisplay { get; set; }
    public required string Method { get; set; }
    public required string MethodDisplay { get; set; }
    public string? Gateway { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? Description { get; set; }
    public string? RefundReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    public required long RefundAmount { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO آمار حسابداری
/// </summary>
public class AccountingStatsDto
{
    public required long TotalRevenue { get; set; }
    public required long MonthlyRevenue { get; set; }
    public required int TotalTransactions { get; set; }
    public required int ActiveStudents { get; set; }
    public required decimal RevenueChange { get; set; }
    public required decimal TransactionChange { get; set; }
    public required decimal StudentChange { get; set; }
    public required List<MonthlyRevenueDto> MonthlyData { get; set; }
    public required List<PaymentMethodStatsDto> PaymentMethodStats { get; set; }
}

/// <summary>
/// DTO داده‌های درآمد ماهانه
/// </summary>
public class MonthlyRevenueDto
{
    public required string Month { get; set; }
    public required long Revenue { get; set; }
    public required int TransactionCount { get; set; }
}

/// <summary>
/// DTO آمار روش‌های پرداخت
/// </summary>
public class PaymentMethodStatsDto
{
    public required string Method { get; set; }
    public required string MethodName { get; set; }
    public required int Count { get; set; }
    public required long Amount { get; set; }
    public required decimal Percentage { get; set; }
}

/// <summary>
/// DTO بازگشت وجه
/// </summary>
public class RefundTransactionDto
{
    public required Guid TransactionId { get; set; }
    public required string Reason { get; set; }
    public long? RefundAmount { get; set; } // اگر null باشد، کل مبلغ بازگردانده می‌شود
}

/// <summary>
/// DTO فیلتر تراکنش‌ها
/// </summary>
public class TransactionFilterDto
{
    public TransactionStatus? Status { get; set; }
    public PaymentMethod? Method { get; set; }
    public string? Gateway { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO تولید گزارش
/// </summary>
public class GenerateReportDto
{
    public required ReportType Type { get; set; }
    public required DateRangeType DateRange { get; set; }
    public required ReportFormat Format { get; set; }
    public DateTime? CustomFromDate { get; set; }
    public DateTime? CustomToDate { get; set; }
}

/// <summary>
/// نوع گزارش
/// </summary>
public enum ReportType
{
    Revenue = 0,    // درآمد
    Students = 1,   // دانشجویان
    Courses = 2,    // دوره‌ها
    Payments = 3    // پرداخت‌ها
}

/// <summary>
/// بازه زمانی گزارش
/// </summary>
public enum DateRangeType
{
    Week = 0,       // هفته
    Month = 1,      // ماه
    Quarter = 2,    // فصل
    Year = 3,       // سال
    Custom = 4      // سفارشی
}

/// <summary>
/// فرمت گزارش
/// </summary>
public enum ReportFormat
{
    Pdf = 0,        // PDF
    Excel = 1,      // Excel
    Csv = 2         // CSV
}