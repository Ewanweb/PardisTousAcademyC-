using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Handler بررسی کامنت توسط ادمین
/// </summary>
public class ReviewCommentHandler : IRequestHandler<ReviewCommentCommand, OperationResult<CourseCommentDto>>
{
    private readonly IRepository<CourseComment> _commentRepository;
    private readonly IMapper _mapper;

    public ReviewCommentHandler(IRepository<CourseComment> commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseCommentDto>> Handle(ReviewCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _commentRepository.Table
                .Include(c => c.Course)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

            if (comment == null)
                return OperationResult<CourseCommentDto>.NotFound("کامنت یافت نشد");

            // بررسی کامنت
            switch (request.Status)
            {
                case CommentStatus.Approved:
                    comment.Approve(request.AdminUserId, request.Note);
                    break;
                case CommentStatus.Rejected:
                    if (string.IsNullOrWhiteSpace(request.Note))
                        return OperationResult<CourseCommentDto>.Error("دلیل رد کامنت باید مشخص شود");
                    comment.Reject(request.AdminUserId, request.Note);
                    break;
                default:
                    return OperationResult<CourseCommentDto>.Error("وضعیت نامعتبر است");
            }

            await _commentRepository.SaveChangesAsync(cancellationToken);

            var commentDto = _mapper.Map<CourseCommentDto>(comment);
            return OperationResult<CourseCommentDto>.Success(commentDto);
        }
        catch (ArgumentException ex)
        {
            return OperationResult<CourseCommentDto>.Error(ex.Message);
        }
        catch (Exception ex)
        {
            return OperationResult<CourseCommentDto>.Error($"خطا در بررسی کامنت: {ex.Message}");
        }
    }
}