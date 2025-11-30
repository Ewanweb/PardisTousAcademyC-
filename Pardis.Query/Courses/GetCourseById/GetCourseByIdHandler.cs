using AutoMapper;
using MediatR;
using Pardis.Domain.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourseById
{
    public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, CourseResource>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public GetCourseByIdHandler(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<CourseResource> Handle(GetCourseByIdQuery request, CancellationToken token)
        {
            var course = await _courseRepository.GetCourseByIdWithDetailsAsync(request.Id, token);

            if (course == null) return null;

            return _mapper.Map<CourseResource>(course);
        }
    }
}
