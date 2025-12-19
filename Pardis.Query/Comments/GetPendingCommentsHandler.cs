using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Query.Comments;

public class GetPendingCommentsHandler : IRequestHandler<GetPendingCommentsQuery, List<CourseCommentDto>>
{
    private readonly IRepository<CourseComment> _commentRepository;
    private readonly IMapper _mapper;

    public GetPendingCommentsHandler(IRepository<CourseComment> commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseCommentDto>> Handle(GetPendingCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.Table
            .Include(c => c.Course)
            .Include(c => c.User)
            .Where(c => c.Status == CommentStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CourseCommentDto>>(comments);
    }
}