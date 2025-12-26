using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;

namespace Pardis.Query.Courses.Enroll;

/// <summary>
/// Handler برای دریافت دوره‌های ثبت‌نام شده یک کاربر
/// </summary>
public class GetUserEnrollmentsHandler(IRepository<UserCourse> context, IMapper mapper) 
    : IRequestHandler<GetUserEnrollmentsQuery, List<CourseResource>>
{
    private readonly IRepository<UserCourse> _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<List<CourseResource>> Handle(GetUserEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        // اعتبارسنجی ورودی
        if (string.IsNullOrWhiteSpace(request.UserId))
            return new List<CourseResource>();

        var enrollments = await _context.Table
            .AsNoTracking() // فقط خواندنی (برای سرعت بیشتر)

            // ✅ لود کردن اطلاعات دوره
            .Include(uc => uc.Course)

            // ✅ لود کردن اطلاعات مدرسِ دوره
            .Include(uc => uc.Course.Instructor)

            // ✅ لود کردن دسته‌بندی دوره
            .Include(uc => uc.Course.Category)

            // ✅ لود کردن سرفصل‌های دوره (برای نمایش در جزئیات اگر لازم باشد)
            .Include(uc => uc.Course.Sections)
            
            // ✅ لود کردن زمان‌بندی‌های دوره
            .Include(uc => uc.Course.Schedules)

            // فیلتر روی کاربر جاری و دوره‌های غیرحذف شده
            .Where(uc => uc.UserId == request.UserId && !uc.Course.IsDeleted)
            
            // مرتب‌سازی بر اساس تاریخ ثبت‌نام (جدیدترین اول)
            .OrderByDescending(uc => uc.EnrolledAt)
            .ToListAsync(cancellationToken);

        // تبدیل به Resource با استفاده از AutoMapper
        // چون در MappingProfile تنظیم کردیم (IncludeMembers)، اطلاعات دوره و کاربر ترکیب می‌شوند
        var result = _mapper.Map<List<CourseResource>>(enrollments);

        return result;
    }
}