using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Shopping;

namespace Pardis.Query.Courses.GetCourseDetail;

/// <summary>
/// Handler برای دریافت جزئیات کامل دوره با اطلاعات دسترسی
/// </summary>
public class GetCourseDetailHandler : IRequestHandler<GetCourseDetailQuery, CourseDetailDto?>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<UserCourse> _userCourseRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMapper _mapper;

    public GetCourseDetailHandler(
        IRepository<Course> courseRepository,
        IRepository<UserCourse> userCourseRepository,
        IRepository<Cart> cartRepository,
        IMapper mapper)
    {
        _courseRepository = courseRepository;
        _userCourseRepository = userCourseRepository;
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<CourseDetailDto?> Handle(GetCourseDetailQuery request, CancellationToken cancellationToken)
    {
        // پیدا کردن دوره بر اساس slug
        var course = await _courseRepository.Table
            .AsNoTracking()
            .Include(c => c.Instructor)
            .Include(c => c.Category)
            .Include(c => c.Sections)
            .Include(c => c.Schedules)
            .FirstOrDefaultAsync(c => c.Slug == request.Slug && !c.IsDeleted, cancellationToken);

        if (course == null)
            return null;

        CourseDetailDto result;

        // اگر کاربر احراز هویت شده و دوره را خریده باشد
        if (request.IsAuthenticated && !string.IsNullOrEmpty(request.UserId))
        {
            var userCourse = await _userCourseRepository.Table
                .AsNoTracking()
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Category)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Sections)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Schedules)
                .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CourseId == course.Id, cancellationToken);

            if (userCourse != null)
            {
                // کاربر دوره را خریده - از UserCourse mapping استفاده کن
                result = _mapper.Map<CourseDetailDto>(userCourse);
            }
            else
            {
                // کاربر دوره را نخریده - از Course mapping استفاده کن
                result = _mapper.Map<CourseDetailDto>(course);
                
                // بررسی وضعیت سبد خرید
                var isInCart = await _cartRepository.Table
                    .AsNoTracking()
                    .AnyAsync(c => c.UserId == request.UserId && 
                                  c.Items.Any(i => i.CourseId == course.Id), cancellationToken);
                
                result.IsInCart = isInCart;
            }
        }
        else
        {
            // کاربر احراز هویت نشده - فقط اطلاعات عمومی
            result = _mapper.Map<CourseDetailDto>(course);
        }

        return result;
    }
}