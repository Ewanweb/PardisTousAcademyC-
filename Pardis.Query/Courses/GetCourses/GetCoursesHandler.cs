using AutoMapper;
using MediatR;
using Pardis.Domain.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, List<CourseResource>>
    {
        private readonly ICourseRepository _repository;
        private readonly IMapper _mapper;

        public GetCoursesHandler(ICourseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CourseResource>> Handle(GetCoursesQuery request, CancellationToken token)
        {
            // تمام پیچیدگی‌های دیتابیس به ریپازیتوری منتقل شده است
            var courses = await _repository.GetCoursesWithFilterAsync(
                request.Trashed,
                request.IsAdminOrManager,
                request.CategoryId,
                token
            );

            return _mapper.Map<List<CourseResource>>(courses);
        }
    }
}
