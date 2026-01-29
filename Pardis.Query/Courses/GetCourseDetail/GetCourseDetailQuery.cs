using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.GetCourseDetail;

/// <summary>
/// Query برای دریافت جزئیات کامل دوره با اطلاعات دسترسی
/// </summary>
public class GetCourseDetailQuery : IRequest<CourseDetailDto?>
{
    /// <summary>
    /// Slug دوره
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// شناسه کاربر جاری (برای بررسی وضعیت خرید)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// آیا کاربر احراز هویت شده است؟
    /// </summary>
    public bool IsAuthenticated { get; set; }
}