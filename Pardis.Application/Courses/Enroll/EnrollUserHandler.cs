using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;

namespace Pardis.Application.Courses.Enroll;

public class EnrollUserHandler : IRequestHandler<EnrollUserCommand, OperationResult<bool>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<UserCourse> _context; // برای دسترسی مستقیم به جدول واسط

    public EnrollUserHandler(IRepository<Course> courseRepository, IRepository<UserCourse> context)
    {
        _courseRepository = courseRepository;
        _context = context;
    }

    public async Task<OperationResult<bool>> Handle(EnrollUserCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return OperationResult<bool>.NotFound("دوره مورد نظر یافت نشد.");
        }

        // 2. بررسی اینکه قبلاً ثبت‌نام کرده یا نه
        var alreadyEnrolled = await _context.Table
            .AnyAsync(uc => uc.UserId == request.UserId && uc.CourseId == request.CourseId, cancellationToken);

        if (alreadyEnrolled)
        {
            return OperationResult<bool>.Error("شما قبلاً در این دوره ثبت‌نام کرده‌اید.");
        }

        // 3. ایجاد رکورد ثبت‌نام
        var enrollment = new UserCourse
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow,
            PurchasePrice = course.Price, // ذخیره قیمت در لحظه خرید
            Status = StudentCourseStatus.Active,

            // مقادیر پیش‌فرض
            AttendedSessionsCount = 0,
            AbsentSessionsCount = 0,
            IsCompleted = false,
            IsAccessBlocked = false
        };

        await _context.AddAsync(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult<bool>.Success(true);
    }
}