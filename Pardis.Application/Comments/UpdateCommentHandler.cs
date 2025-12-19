using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Comments;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Handler بروزرسانی کامنت
/// </summary>
public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, OperationResult<CourseCommentDto>>
{
    private readonly IRepository<CourseComment> _commentRepository;
    private readonly IMapper _mapper;

    public UpdateCommentHandler(IRepository<CourseComment> commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseCommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _commentRepository.Table
                .Include(c => c.Course)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId && c.UserId == request.UserId, cancellationToken);

            if (comment == null)
                return OperationResult<CourseCommentDto>.NotFound("کامنت یافت نشد یا شما مجاز به ویرایش آن نیستید");

            // فقط کامنت‌های در انتظار تأیید قابل ویرایش هستند
            if (comment.Status != CommentStatus.Pending)
                return OperationResult<CourseCommentDto>.Error("فقط کامنت‌های در انتظار تأیید قابل ویرایش هستند");

            // بروزرسانی کامنت
            comment.UpdateContent(request.Content, request.Rating);

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
            return OperationResult<CourseCommentDto>.Error($"خطا در بروزرسانی کامنت: {ex.Message}");
        }
    }
}