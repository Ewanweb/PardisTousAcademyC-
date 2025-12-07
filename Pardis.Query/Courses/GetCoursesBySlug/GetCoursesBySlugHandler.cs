using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCoursesBySlug
{
    public class GetCoursesBySlugHandler : IRequestHandler<GetCoursesBySlugQuery, CourseResource>
    {
        private readonly IRepository<Course> _repository;
        private readonly IMapper _mapper;
        public GetCoursesBySlugHandler(IRepository<Course> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CourseResource> Handle(GetCoursesBySlugQuery request, CancellationToken cancellationToken)
        {
            var course = _repository.Table
                .Include(c => c.Seo)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections.OrderBy(s => s.Order))
                .FirstOrDefault(x => x.Slug == request.Slug);

            if (course == null || course.Status is not CourseStatus.Published)
                return null;

            var result = _mapper.Map<CourseResource>(course);

            return result;
                
        }
    }
}
