using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Dto.Comments;
using Pardis.Domain.Comments;
using Pardis.Domain.Courses;

namespace Pardis.Query.Comments;

/// <summary>
/// Handler برای دریافت آمار کامنت‌های یک دوره
/// </summary>
public class GetCourseCommentStatsHandler : IRequestHandler<GetCourseCommentStatsQuery, CourseCommentStatsDto?>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IRepository<CourseComment> _commentRepository;

    public GetCourseCommentStatsHandler(
        ICourseRepository courseRepository,
        IRepository<CourseComment> commentRepository)
    {
        _courseRepository = courseRepository;
        _commentRepository = commentRepository;
    }

    public async Task<CourseCommentStatsDto?> Handle(GetCourseCommentStatsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
            return null;

        var comments = await _commentRepository.Table
            .Where(c => c.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        var totalComments = comments.Count;
        var pendingComments = comments.Count(c => c.Status == CommentStatus.Pending);
        var approvedComments = comments.Count(c => c.Status == CommentStatus.Approved);
        var rejectedComments = comments.Count(c => c.Status == CommentStatus.Rejected);

        // محاسبه میانگین امتیاز (فقط کامنت‌های تأیید شده)
        var approvedRatings = comments
            .Where(c => c.Status == CommentStatus.Approved && c.Rating > 0)
            .Select(c => c.Rating)
            .ToList();

        var averageRating = approvedRatings.Any() ? approvedRatings.Average() : 0;

        var stats = new CourseCommentStatsDto
        {
            CourseId = request.CourseId,
            TotalComments = totalComments,
            PendingComments = pendingComments,
            ApprovedComments = approvedComments,
            RejectedComments = rejectedComments,
            AverageRating = Math.Round(averageRating, 1)
        };

        return stats;
    }
}