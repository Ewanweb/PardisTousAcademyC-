using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Api.Controllers
{
    public class LogEntry
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // فقط کاربران احراز هویت شده
    public class LogsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public LogsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// دریافت لیست فایل‌های لاگ
        /// </summary>
        [HttpGet("files")]
        public IActionResult GetLogFiles()
        {
            try
            {
                var logsPath = Path.Combine(_environment.ContentRootPath, "Logs");
                
                if (!Directory.Exists(logsPath))
                {
                    return Ok(new { files = new string[0], message = "پوشه لاگ وجود ندارد" });
                }

                var files = Directory.GetFiles(logsPath, "*.txt")
                    .Select(f => new
                    {
                        name = Path.GetFileName(f),
                        size = new FileInfo(f).Length,
                        lastModified = new FileInfo(f).LastWriteTime,
                        path = f
                    })
                    .OrderByDescending(f => f.lastModified)
                    .ToList();

                return Ok(new { files });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"خطا در دریافت فایل‌های لاگ: {ex.Message}" });
            }
        }

        /// <summary>
        /// دریافت محتوای فایل لاگ
        /// </summary>
        [HttpGet("content/{fileName}")]
        public async Task<IActionResult> GetLogContent(string fileName, [FromQuery] int lines = 100)
        {
            try
            {
                var logsPath = Path.Combine(_environment.ContentRootPath, "Logs");
                var filePath = Path.Combine(logsPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { error = "فایل لاگ یافت نشد" });
                }

                // خواندن آخرین خطوط فایل
                var allLines = await System.IO.File.ReadAllLinesAsync(filePath);
                var lastLines = allLines.TakeLast(lines).ToArray();

                return Ok(new 
                { 
                    fileName,
                    totalLines = allLines.Length,
                    displayedLines = lastLines.Length,
                    content = string.Join("\n", lastLines)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"خطا در خواندن فایل لاگ: {ex.Message}" });
            }
        }

        /// <summary>
        /// جستجو در لاگ‌ها
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchLogs([FromQuery] string query, [FromQuery] string? fileName = null, [FromQuery] int maxResults = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { error = "متن جستجو الزامی است" });
                }

                var logsPath = Path.Combine(_environment.ContentRootPath, "Logs");
                
                if (!Directory.Exists(logsPath))
                {
                    return Ok(new { results = new object[0], message = "پوشه لاگ وجود ندارد" });
                }

                var filesToSearch = string.IsNullOrEmpty(fileName) 
                    ? Directory.GetFiles(logsPath, "*.txt")
                    : new[] { Path.Combine(logsPath, fileName) };

                var results = new List<object>();

                foreach (var file in filesToSearch.Where(System.IO.File.Exists))
                {
                    var lines = await System.IO.File.ReadAllLinesAsync(file);
                    var matchingLines = lines
                        .Select((line, index) => new { line, lineNumber = index + 1 })
                        .Where(x => x.line.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Take(maxResults)
                        .Select(x => new
                        {
                            fileName = Path.GetFileName(file),
                            lineNumber = x.lineNumber,
                            content = x.line.Trim()
                        });

                    results.AddRange(matchingLines);
                }

                return Ok(new 
                { 
                    query,
                    totalResults = results.Count,
                    results = results.Take(maxResults)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"خطا در جستجوی لاگ‌ها: {ex.Message}" });
            }
        }

        /// <summary>
        /// دریافت لاگ‌های اخیر بر اساس سطح
        /// </summary>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentLogs([FromQuery] string level = "Error", [FromQuery] int hours = 24, [FromQuery] int maxResults = 100)
        {
            try
            {
                var logsPath = Path.Combine(_environment.ContentRootPath, "Logs");
                
                if (!Directory.Exists(logsPath))
                {
                    return Ok(new { logs = new object[0], message = "پوشه لاگ وجود ندارد" });
                }

                var cutoffTime = DateTime.Now.AddHours(-hours);
                var files = Directory.GetFiles(logsPath, "*.txt")
                    .Where(f => new FileInfo(f).LastWriteTime >= cutoffTime)
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime);

                var logs = new List<LogEntry>();

                foreach (var file in files)
                {
                    var lines = await System.IO.File.ReadAllLinesAsync(file);
                    var matchingLines = lines
                        .Where(line => line.Contains($"[{level.ToUpper()}]", StringComparison.OrdinalIgnoreCase))
                        .Select(line => new LogEntry
                        {
                            FileName = Path.GetFileName(file),
                            Timestamp = ExtractTimestamp(line),
                            Level = level,
                            Content = line.Trim()
                        })
                        .Where(log => log.Timestamp >= cutoffTime);

                    logs.AddRange(matchingLines);
                }

                var sortedLogs = logs
                    .OrderByDescending(log => log.Timestamp)
                    .Take(maxResults);

                return Ok(new 
                { 
                    level,
                    hours,
                    totalFound = logs.Count,
                    logs = sortedLogs.Select(log => new
                    {
                        fileName = log.FileName,
                        timestamp = log.Timestamp,
                        level = log.Level,
                        content = log.Content
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"خطا در دریافت لاگ‌های اخیر: {ex.Message}" });
            }
        }

        private DateTime ExtractTimestamp(string logLine)
        {
            try
            {
                // استخراج timestamp از فرمت: "2024-01-01 12:00:00.000 +00:00"
                var parts = logLine.Split(' ');
                if (parts.Length >= 2)
                {
                    var dateStr = parts[0];
                    var timeStr = parts[1].Split('.')[0]; // حذف میلی‌ثانیه
                    
                    if (DateTime.TryParse($"{dateStr} {timeStr}", out var result))
                    {
                        return result;
                    }
                }
            }
            catch
            {
                // در صورت خطا، زمان فعلی را برگردان
            }

            return DateTime.MinValue;
        }
    }
}