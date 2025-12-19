using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Query.Comments;

public class GetCourseCommentsHandler : IRequestHandler<GetCourseCommentsQuery, List<CourseCommentDto>>
{
    private readonly IRepository<CourseComment> _commentRepository;
    private readonly IMapper _mapper;

    public GetCourseCommentsHandler(IRepository<CourseComment> commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseCommentDto>> Handle(GetCourseCommentsQuery request, CancellationToken cancellationToken)
    {
        var query = _commentRepository.Table
            .Include(c => c.Course)
            .Include(c => c.User)
            .Include(c => c.ReviewedByUser)
            .Where(c => c.CourseId == request.CourseId);

        // فیلتر بر اساس وضعیت
        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }
        else if (!request.IncludeRejected)
        {
            // اگر وضعیت مشخص نشده و نباید رد شده‌ها نمایش داده شوند
            query = query.Where(c => c.Status != CommentStatus.Rejected);
        }

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CourseCommentDto>>(comments);
    }
}