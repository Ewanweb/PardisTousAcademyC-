using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;

namespace Pardis.Query.Courses.GetMyCourses;

/// <summary>
/// Handler for getting instructor's courses
/// </summary>
public class GetMyCoursesHandler : IRequestHandler<GetMyCoursesQuery, GetMyCoursesResult>
{
    private readonly IRepository<Course> _courseRepository;

    public GetMyCoursesHandler(IRepository<Course> courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<GetMyCoursesResult> Handle(GetMyCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = _courseRepository.Table
            .Include(c => c.Students)
            .AsNoTracking()
            .AsQueryable();

        // Role-based filtering
        if (request.UserRole == Role.Instructor)
        {
            // Instructor can only see their own courses
            query = query.Where(c => c.InstructorId == request.UserId);
        }
        else if (request.UserRole == Role.EducationExpert || request.UserRole == Role.CourseSupport)
        {
            // EducationExpert and CourseSupport can see all courses
            // No additional filtering needed
        }
        else
        {
            // Other roles should not access this endpoint, but just in case
            query = query.Where(c => c.InstructorId == request.UserId);
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(searchTerm) || 
                                   c.Slug.ToLower().Contains(searchTerm));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<CourseStatus>(request.Status, true, out var status))
            {
                query = query.Where(c => c.Status == status);
            }
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.Sort?.ToLower() switch
        {
            "title" => query.OrderBy(c => c.Title),
            "title_desc" => query.OrderByDescending(c => c.Title),
            "created" => query.OrderBy(c => c.CreatedAt),
            "created_desc" => query.OrderByDescending(c => c.CreatedAt),
            "students" => query.OrderBy(c => c.Students.Count),
            "students_desc" => query.OrderByDescending(c => c.Students.Count),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        // Apply pagination
        var courses = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var items = courses.Select(c => new MyCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Code = c.Slug, // Using slug as code for now
            IsActive = c.Status == CourseStatus.Published,
            StudentsCount = c.Students.Count,
            LastActivityAt = c.UpdatedAt
        }).ToList();

        return new GetMyCoursesResult
        {
            Items = items,
            Pagination = new PaginationDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = totalCount
            }
        };
    }
}