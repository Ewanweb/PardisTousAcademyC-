using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;

namespace Pardis.Application.Courses.Enroll;

public class EnrollUserHandler : IRequestHandler<EnrollUserCommand, OperationResult<bool>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseEnrollment> _enrollmentRepository;
    private readonly IRepository<UserCourse> _userCourseRepository;

    public EnrollUserHandler(
        IRepository<Course> courseRepository, 
        IRepository<CourseEnrollment> enrollmentRepository,
        IRepository<UserCourse> userCourseRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _userCourseRepository = userCourseRepository;
    }

    public async Task<OperationResult<bool>> Handle(EnrollUserCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی وجود دوره
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return OperationResult<bool>.NotFound("دوره مورد نظر یافت نشد.");
        }

        // 2. بررسی اینکه قبلاً ثبت‌نام کرده یا نه (در هر دو جدول)
        var alreadyEnrolledInNew = await _enrollmentRepository.Table
            .AnyAsync(e => e.StudentId == request.UserId && e.CourseId == request.CourseId, cancellationToken);

        var alreadyEnrolledInOld = await _userCourseRepository.Table
            .AnyAsync(uc => uc.UserId == request.UserId && uc.CourseId == request.CourseId, cancellationToken);

        if (alreadyEnrolledInNew || alreadyEnrolledInOld)
        {
            return OperationResult<bool>.Error("شما قبلاً در این دوره ثبت‌نام کرده‌اید.");
        }

        // 3. ایجاد رکورد ثبت‌نام در جدول جدید (CourseEnrollment)
        var enrollment = new CourseEnrollment(
            courseId: request.CourseId,
            studentId: request.UserId,
            totalAmount: course.Price,
            isInstallmentAllowed: false,
            installmentCount: null
        );

        await _enrollmentRepository.AddAsync(enrollment);

        // 4. ایجاد رکورد در جدول قدیمی (UserCourse) برای سازگاری با کدهای قدیمی
        var userCourse = new UserCourse
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow,
            PurchasePrice = course.Price,
            Status = StudentCourseStatus.Active,
            AttendedSessionsCount = 0,
            AbsentSessionsCount = 0,
            IsCompleted = false,
            IsAccessBlocked = false
        };

        await _userCourseRepository.AddAsync(userCourse);

        // 5. ذخیره تغییرات
        await _enrollmentRepository.SaveChangesAsync(cancellationToken);
        await _userCourseRepository.SaveChangesAsync(cancellationToken);

        return OperationResult<bool>.Success(true);
    }
}