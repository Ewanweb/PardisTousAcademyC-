using MediatR;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Enroll
{
    /// <summary>
    /// Query برای دریافت دوره‌های ثبت‌نام شده یک کاربر
    /// </summary>
    public class GetUserEnrollmentsQuery : IRequest<List<CourseResource>>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required string UserId { get; set; }
    }
}
