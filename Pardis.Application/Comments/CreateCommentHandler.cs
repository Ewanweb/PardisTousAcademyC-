using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Comments;
using Pardis.Domain.Courses;
using Pardis.Domain.Dto.Comments;

namespace Pardis.Application.Comments;

/// <summary>
/// Handler ایجاد کامنت دوره
/// </summary>
public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, OperationResult<CourseCommentDto>>
{
    private readonly IRepository<CourseComment> _commentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<UserCourse> _userCourseRepository;
    private readonly IMapper _mapper;

    public CreateCommentHandler(
        IRepository<CourseComment> commentRepository,
        IRepository<Course> courseRepository,
        IRepository<UserCourse> userCourseRepository,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _courseRepository = courseRepository;
        _userCourseRepository = userCourseRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<CourseCommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی وجود دوره
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return OperationResult<CourseCommentDto>.NotFound("دوره یافت نشد");

            // بررسی ثبت‌نام دانشجو در دوره
            var enrollment = await _userCourseRepository.Table
                .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CourseId == request.CourseId, cancellationToken);
            
            if (enrollment == null)
                return OperationResult<CourseCommentDto>.Error("فقط دانشجویان ثبت‌نام‌شده می‌توانند کامنت ثبت کنند");

            // بررسی وجود کامنت قبلی از همین کاربر
            var existingComment = await _commentRepository.Table
                .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.CourseId == request.CourseId, cancellationToken);
            
            if (existingComment != null)
                return OperationResult<CourseCommentDto>.Error("شما قبلاً برای این دوره کامنت ثبت کرده‌اید");

            // ایجاد کامنت جدید
            var comment = new CourseComment(request.CourseId, request.UserId, request.Content, request.Rating);

            await _commentRepository.AddAsync(comment);
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
            return OperationResult<CourseCommentDto>.Error($"خطا در ثبت کامنت: {ex.Message}");
        }
    }
}