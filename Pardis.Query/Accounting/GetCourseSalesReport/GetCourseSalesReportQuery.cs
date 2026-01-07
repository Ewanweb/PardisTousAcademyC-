using MediatR;

namespace Pardis.Query.Accounting.GetCourseSalesReport;

/// <summary>
/// Query برای دریافت گزارش فروش دوره‌ها
/// </summary>
public class GetCourseSalesReportQuery : IRequest<GetCourseSalesReportResult>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "Revenue"; // Revenue, SalesCount, CourseName
    public bool SortDescending { get; set; } = true;
}

/// <summary>
/// نتیجه گزارش فروش دوره‌ها
/// </summary>
public class GetCourseSalesReportResult
{
    public List<CourseSalesDto> CourseSales { get; set; } = new();
    public int TotalCount { get; set; }
    public long TotalRevenue { get; set; }
    public int TotalSales { get; set; }
}

public class CourseSalesDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseSlug { get; set; } = string.Empty;
    public string? InstructorName { get; set; }
    public long CoursePrice { get; set; }
    public int SalesCount { get; set; }
    public long TotalRevenue { get; set; }
    public long AveragePrice { get; set; }
    public DateTime? FirstSaleDate { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public int ActiveStudents { get; set; }
    public int CompletedStudents { get; set; }
}