using Pardis.Domain.Courses;

namespace Pardis.Application.Courses.Contracts;

/// <summary>
/// Course repository interface for application layer
/// </summary>
public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<Course>> GetByInstructorIdAsync(string instructorId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
}