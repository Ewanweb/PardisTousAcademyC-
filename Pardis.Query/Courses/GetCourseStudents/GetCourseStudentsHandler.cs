using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Courses.GetCourseStudents;

/// <summary>
/// Handler for getting course students
/// </summary>
public class GetCourseStudentsHandler : IRequestHandler<GetCourseStudentsQuery, GetCourseStudentsResult>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseEnrollment> _enrollmentRepository;

    public GetCourseStudentsHandler(
        IRepository<Course> courseRepository,
        IRepository<CourseEnrollment> enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<GetCourseStudentsResult> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        // First, check if course exists and user has permission
        var course = await _courseRepository.Table
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course == null)
        {
            throw new ArgumentException("Course not found", nameof(request.CourseId));
        }

        // Check permissions
        var hasPermission = request.UserRole switch
        {
            Role.EducationExpert or Role.CourseSupport => true,
            Role.Instructor => course.InstructorId == request.UserId,
            _ => false
        };

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("You don't have permission to view students for this course");
        }

        // Build query for enrollments
        var query = _enrollmentRepository.Table
            .Include(e => e.Student)
            .Where(e => e.CourseId == request.CourseId)
            .AsNoTracking()
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(e => e.Student.FullName!.ToLower().Contains(searchTerm) ||
                                   e.Student.Email!.ToLower().Contains(searchTerm) ||
                                   e.Student.PhoneNumber!.ToLower().Contains(searchTerm));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<EnrollmentStatus>(request.Status, true, out var status))
            {
                query = query.Where(e => e.EnrollmentStatus == status);
            }
        }

        // Date range filter
        if (request.From.HasValue)
        {
            query = query.Where(e => e.EnrollmentDate >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(e => e.EnrollmentDate <= request.To.Value);
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Calculate statistics
        var allEnrollments = await _enrollmentRepository.Table
            .Where(e => e.CourseId == request.CourseId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var stats = new CourseStatsDto
        {
            TotalStudents = allEnrollments.Count,
            Active = allEnrollments.Count(e => e.EnrollmentStatus == EnrollmentStatus.Active),
            Completed = allEnrollments.Count(e => e.EnrollmentStatus == EnrollmentStatus.Completed),
            Dropped = allEnrollments.Count(e => e.EnrollmentStatus == EnrollmentStatus.Cancelled),
            AvgProgress = allEnrollments.Any() ? allEnrollments.Average(e => e.GetPaymentPercentage()) : 0
        };

        // Apply sorting
        query = request.Sort?.ToLower() switch
        {
            "name" => query.OrderBy(e => e.Student.FullName),
            "name_desc" => query.OrderByDescending(e => e.Student.FullName),
            "enrolled" => query.OrderBy(e => e.EnrollmentDate),
            "enrolled_desc" => query.OrderByDescending(e => e.EnrollmentDate),
            "progress" => query.OrderBy(e => e.PaidAmount),
            "progress_desc" => query.OrderByDescending(e => e.PaidAmount),
            _ => query.OrderByDescending(e => e.EnrollmentDate)
        };

        // Apply pagination
        var enrollments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var items = enrollments.Select(e => new CourseStudentDto
        {
            UserId = e.StudentId,
            FullName = e.Student.FullName ?? "Unknown",
            Mobile = e.Student.PhoneNumber,
            Email = e.Student.Email,
            EnrolledAt = e.EnrollmentDate,
            Status = e.EnrollmentStatus.ToString(),
            ProgressPercent = e.GetPaymentPercentage(),
            LastSeenAt = e.UpdatedAt // Using UpdatedAt as last activity indicator
        }).ToList();

        return new GetCourseStudentsResult
        {
            Course = new CourseInfoDto
            {
                Id = course.Id,
                Title = course.Title
            },
            Stats = stats,
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