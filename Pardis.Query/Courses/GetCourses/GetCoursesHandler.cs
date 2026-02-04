using Pardis.Domain.Dto.Seo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, List<CourseResource>>
    {
        private readonly IRepository<Course> _repository;

        public GetCoursesHandler(IRepository<Course> repository)
        {
            _repository = repository;
        }

        public async Task<List<CourseResource>> Handle(GetCoursesQuery request, CancellationToken token)
        {
            // ? ??????????: ???? ???? ???? ??? ??????? ?????
            var query = _repository.Table
                .Include(c => c.Instructor) // ??? instructor
                .Include(c => c.Category)   // ??? category
                .AsNoTracking() // ???? ???? ?????
                .AsQueryable();

            // ????? ??? ?????
            if (request.Trashed)
            {
                query = query.IgnoreQueryFilters().Where(c => c.IsDeleted);
            }
            else
            {
                query = query.Where(c => !c.IsDeleted);
            }

            // ????? ?????????
            if (request.CategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == request.CategoryId);
            }

            // ????? ??????
            if (request.IsAdminOrManager)
            {
                // ????? ??? ?? ???????
            }
            else if (request.IsInstructor && !string.IsNullOrEmpty(request.CurrentUserId))
            {
                // ???? ??? ???????? ????
                query = query.Where(c => c.InstructorId == request.CurrentUserId);
            }
            else
            {
                // ????? ???? ??? ????? ??????
                query = query.Where(c => c.Status == CourseStatus.Published);
            }

            // ? ??????????: ????????? ???? ???? ??? performance
            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(token);

            // ? ????? ???? ?? Resource (???? ??????? ?????)
            var result = courses.Select(c => new CourseResource
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Description = c.Description,
                Price = c.Price,
                Status = c.Status.ToString(),
                Type = c.Type.ToString(),
                Location = c.Location,
                Thumbnail = c.Thumbnail ?? "",
                StartFrom = c.StartFrom,
                Schedule = c.Schedule,
                IsCompleted = c.IsCompleted,
                IsStarted = c.IsStarted,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,

                // ? ??? ??????? ????? instructor
                Instructor = c.Instructor != null ? new InstructorBasicDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FullName ?? c.Instructor.UserName ?? "",
                    Email = c.Instructor.Email ?? "",
                    Mobile = c.Instructor.PhoneNumber
                } : null,

                // ? ??? ??????? ????? category
                Category = c.Category != null ? new CategoryResource
                {
                    Id = c.Category.Id,
                    Title = c.Category.Title,
                    Slug = c.Category.Slug,
                    CoursesCount = c.Category.CoursesCount
                } : null,

                // ? ???? ???? ???? ?????? ???? ???? - empty lists
                Sections = new List<CourseSectionDto>(),
                Seo = new SeoDto(),
                Schedules = new List<CourseScheduleDto>()

            }).ToList();

            return result;
        }
    }
}
