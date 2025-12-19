using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Dto.Accounting;
using Pardis.Domain.Users;
using Api.Controllers;

namespace Api.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت گزارش‌های مالی
/// </summary>
[Route("api/admin/reports")]
[Authorize(Roles = Role.Admin + "," + Role.Manager + "," + "GeneralManager" + "," + "FinancialManager")]
public class ReportsController : BaseController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator, ILogger<ReportsController> logger) : base(logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// تولید گزارش مالی
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // برای الان یک پاسخ نمونه برمی‌گردانیم
            var reportUrl = GenerateReportFile(dto);
            
            return SuccessResponse(new { downloadUrl = reportUrl }, "گزارش با موفقیت تولید شد");
        }, "خطا در تولید گزارش");
    }

    /// <summary>
    /// دریافت لیست گزارش‌های موجود
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: پیاده‌سازی دریافت لیست گزارش‌ها
            var reports = new List<object>
            {
                new { id = 1, name = "گزارش درآمد ماهانه", type = "revenue", createdAt = DateTime.Now.AddDays(-1) },
                new { id = 2, name = "گزارش دانشجویان", type = "students", createdAt = DateTime.Now.AddDays(-2) },
                new { id = 3, name = "گزارش دوره‌ها", type = "courses", createdAt = DateTime.Now.AddDays(-3) }
            };

            return SuccessResponse(reports, "لیست گزارش‌ها با موفقیت دریافت شد");
        }, "خطا در دریافت لیست گزارش‌ها");
    }

    /// <summary>
    /// دانلود گزارش
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadReport(int id)
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: پیاده‌سازی دانلود گزارش
            // برای الان یک فایل نمونه برمی‌گردانیم
            
            var fileContent = System.Text.Encoding.UTF8.GetBytes("نمونه گزارش مالی");
            var fileName = $"financial_report_{id}_{DateTime.Now:yyyyMMdd}.txt";
            
            return File(fileContent, "text/plain", fileName);
        }, "خطا در دانلود گزارش");
    }

    private static string GenerateReportFile(GenerateReportDto dto)
    {
        var typeStr = dto.Type.ToString().ToLower();
        var formatStr = dto.Format.ToString().ToLower();
        var fileName = $"report_{typeStr}_{DateTime.Now:yyyyMMddHHmmss}.{formatStr}";
        var downloadUrl = $"/api/admin/reports/download/{fileName}";
        
        return downloadUrl;
    }
}