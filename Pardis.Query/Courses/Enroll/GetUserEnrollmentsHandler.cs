using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;

namespace Pardis.Query.Courses.Enroll;

public class GetUserEnrollmentsHandler : IRequestHandler<GetUserEnrollmentsQuery, List<CourseResource>>
{
    private readonly IRepository<UserCourse> _context;
    private readonly IMapper _mapper;

    public GetUserEnrollmentsHandler(IRepository<UserCourse> context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CourseResource>> Handle(GetUserEnrollmentsQuery request, CancellationToken cancellationToken)
    {
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

            // فیلتر روی کاربر جاری
            .Where(uc => uc.UserId == request.UserId)
            .OrderByDescending(uc => uc.EnrolledAt)
            .ToListAsync(cancellationToken);

        // 2. تبدیل به Resource با استفاده از AutoMapper
        // چون در MappingProfile تنظیم کردیم (IncludeMembers)، اطلاعات دوره و کاربر ترکیب می‌شوند
        var result = _mapper.Map<List<CourseResource>>(enrollments);

        return result;

    }
}