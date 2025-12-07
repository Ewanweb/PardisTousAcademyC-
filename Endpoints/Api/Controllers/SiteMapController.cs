using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;
using MediatR;
using Pardis.Domain.Courses;
using Pardis.Infrastructure;

namespace Api.Controllers
{
    [Route("sitemap.xml")]
    [ApiController]
    public class SiteMapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SiteMapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSitemap()
        {
            // 1. آدرس دامنه فرانت‌‌اند (چون لینک‌ها باید به ری‌اکت اشاره کنند)
            string baseUrl = "http://localhost:5173";

            // 2. دریافت تمام دوره‌های فعال
            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Published && !c.IsDeleted)
                .Select(c => new { c.Slug, c.UpdatedAt })
                .ToListAsync();

            // 3. دریافت تمام دسته‌بندی‌ها
            var categories = await _context.Categories
                .Select(c => new { c.Slug })
                .ToListAsync();

            // 4. ساخت XML
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XElement(xmlns + "urlset");

            // افزودن صفحه اصلی
            sitemap.Add(new XElement(xmlns + "url",
                new XElement(xmlns + "loc", $"{baseUrl}/"),
                new XElement(xmlns + "changefreq", "daily"),
                new XElement(xmlns + "priority", "1.0")
            ));

            // افزودن دوره‌ها
            foreach (var course in courses)
            {
                sitemap.Add(new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", $"{baseUrl}/course/{course.Slug}"),
                    new XElement(xmlns + "lastmod", course.UpdatedAt.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd")),
                    new XElement(xmlns + "changefreq", "weekly"),
                    new XElement(xmlns + "priority", "0.8")
                ));
            }

            // افزودن دسته‌بندی‌ها
            foreach (var cat in categories)
            {
                sitemap.Add(new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", $"{baseUrl}/courses/{cat.Slug}"),
                    new XElement(xmlns + "changefreq", "weekly"),
                    new XElement(xmlns + "priority", "0.7")
                ));
            }

            // 5. بازگرداندن فایل
            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
