using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Accounting;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;

namespace Pardis.Query.Accounting.GetCourseSalesReport;

/// <summary>
/// Handler برای دریافت گزارش فروش دوره‌ها
/// </summary>
public class GetCourseSalesReportHandler : IRequestHandler<GetCourseSalesReportQuery, GetCourseSalesReportResult>
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseEnrollment> _enrollmentRepository;

    public GetCourseSalesReportHandler(
        IRepository<Transaction> transactionRepository,
        IRepository<Course> courseRepository,
        IRepository<CourseEnrollment> enrollmentRepository)
    {
        _transactionRepository = transactionRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<GetCourseSalesReportResult> Handle(GetCourseSalesReportQuery request, CancellationToken cancellationToken)
    {
        // Query پایه برای تراکنش‌های تکمیل شده
        var transactionsQuery = _transactionRepository.Table
            .Include(t => t.Course)
            .ThenInclude(c => c.Instructor)
            .Where(t => t.Status == TransactionStatus.Completed);

        // اعمال فیلتر تاریخ
        if (request.FromDate.HasValue)
            transactionsQuery = transactionsQuery.Where(t => t.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            transactionsQuery = transactionsQuery.Where(t => t.CreatedAt <= request.ToDate.Value);

        // اعمال فیلتر جستجو
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            transactionsQuery = transactionsQuery.Where(t => 
                t.Course.Title.ToLower().Contains(searchTerm) ||
                t.Course.Instructor.FullName!.ToLower().Contains(searchTerm));
        }

        // گروه‌بندی بر اساس دوره و محاسبه آمار
        var courseSalesData = await transactionsQuery
            .GroupBy(t => new { t.CourseId, t.Course })
            .Select(g => new
            {
                CourseId = g.Key.CourseId,
                Course = g.Key.Course,
                SalesCount = g.Count(),
                TotalRevenue = g.Sum(t => t.Amount),
                AveragePrice = g.Average(t => t.Amount),
                FirstSaleDate = g.Min(t => t.CreatedAt),
                LastSaleDate = g.Max(t => t.CreatedAt)
            })
            .ToListAsync(cancellationToken);

        // دریافت آمار ثبت‌نام برای هر دوره
        var courseIds = courseSalesData.Select(c => c.CourseId).ToList();
        var enrollmentStats = await _enrollmentRepository.Table
            .Where(e => courseIds.Contains(e.CourseId))
            .GroupBy(e => e.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                ActiveStudents = g.Count(e => e.EnrollmentStatus == EnrollmentStatus.Active),
                CompletedStudents = g.Count(e => e.EnrollmentStatus == EnrollmentStatus.Completed)
            })
            .ToListAsync(cancellationToken);

        // ترکیب داده‌ها
        var courseSales = courseSalesData.Select(c =>
        {
            var enrollmentStat = enrollmentStats.FirstOrDefault(e => e.CourseId == c.CourseId);
            return new CourseSalesDto
            {
                CourseId = c.CourseId,
                CourseTitle = c.Course.Title,
                CourseSlug = c.Course.Slug,
                InstructorName = c.Course.Instructor?.FullName,
                CoursePrice = c.Course.Price,
                SalesCount = c.SalesCount,
                TotalRevenue = c.TotalRevenue,
                AveragePrice = (long)c.AveragePrice,
                FirstSaleDate = c.FirstSaleDate,
                LastSaleDate = c.LastSaleDate,
                ActiveStudents = enrollmentStat?.ActiveStudents ?? 0,
                CompletedStudents = enrollmentStat?.CompletedStudents ?? 0
            };
        }).ToList();

        // مرتب‌سازی
        courseSales = ApplySorting(courseSales, request.SortBy, request.SortDescending);

        // محاسبه آمار کلی
        var totalCount = courseSales.Count;
        var totalRevenue = courseSales.Sum(c => c.TotalRevenue);
        var totalSales = courseSales.Sum(c => c.SalesCount);

        // صفحه‌بندی
        var pagedCourseSales = courseSales
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new GetCourseSalesReportResult
        {
            CourseSales = pagedCourseSales,
            TotalCount = totalCount,
            TotalRevenue = totalRevenue,
            TotalSales = totalSales
        };
    }

    private static List<CourseSalesDto> ApplySorting(List<CourseSalesDto> courseSales, string sortBy, bool sortDescending)
    {
        return sortBy.ToLower() switch
        {
            "revenue" => sortDescending 
                ? courseSales.OrderByDescending(c => c.TotalRevenue).ToList()
                : courseSales.OrderBy(c => c.TotalRevenue).ToList(),
            
            "salescount" => sortDescending 
                ? courseSales.OrderByDescending(c => c.SalesCount).ToList()
                : courseSales.OrderBy(c => c.SalesCount).ToList(),
            
            "coursename" => sortDescending 
                ? courseSales.OrderByDescending(c => c.CourseTitle).ToList()
                : courseSales.OrderBy(c => c.CourseTitle).ToList(),
            
            "price" => sortDescending 
                ? courseSales.OrderByDescending(c => c.CoursePrice).ToList()
                : courseSales.OrderBy(c => c.CoursePrice).ToList(),
            
            "students" => sortDescending 
                ? courseSales.OrderByDescending(c => c.ActiveStudents).ToList()
                : courseSales.OrderBy(c => c.ActiveStudents).ToList(),
            
            _ => sortDescending 
                ? courseSales.OrderByDescending(c => c.TotalRevenue).ToList()
                : courseSales.OrderBy(c => c.TotalRevenue).ToList()
        };
    }
}