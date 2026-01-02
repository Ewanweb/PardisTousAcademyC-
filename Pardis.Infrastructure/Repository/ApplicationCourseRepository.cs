using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Courses;
using AppCourseRepo = Pardis.Application.Courses.Contracts.ICourseRepository;

namespace Pardis.Infrastructure.Repository;

/// <summary>
/// Implementation of ICourseRepository for Application layer
/// </summary>
public class ApplicationCourseRepository : AppCourseRepo
{
    private readonly AppDbContext _context;

    public ApplicationCourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
    }

    public async Task<List<Course>> GetByInstructorIdAsync(string instructorId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Category)
            .Where(c => c.InstructorId == instructorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses.AnyAsync(c => c.Id == courseId, cancellationToken);
    }
}