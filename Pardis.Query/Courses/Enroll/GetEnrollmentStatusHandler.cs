using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Courses;

namespace Pardis.Query.Courses.Enroll;

/// <summary>
/// Handler برای بررسی وضعیت ثبت‌نام کاربر در یک دوره
/// </summary>
public class GetEnrollmentStatusHandler(IRepository<UserCourse> context) 
    : IRequestHandler<GetEnrollmentStatusQuery, EnrollmentStatusResult>
{
    private readonly IRepository<UserCourse> _context = context;

    public async Task<EnrollmentStatusResult> Handle(GetEnrollmentStatusQuery request, CancellationToken cancellationToken)
    {
        // اعتبارسنجی ورودی
        if (string.IsNullOrWhiteSpace(request.UserId) || request.CourseId == Guid.Empty)
        {
            return new EnrollmentStatusResult
            {
                IsEnrolled = false,
                PaymentStatus = "Invalid"
            };
        }

        var enrollment = await _context.Table
            .AsNoTracking()
            .Include(uc => uc.Course)
            .FirstOrDefaultAsync(uc => 
                uc.UserId == request.UserId && 
                uc.CourseId == request.CourseId && 
                !uc.Course.IsDeleted, 
                cancellationToken);

        if (enrollment == null)
        {
            return new EnrollmentStatusResult
            {
                IsEnrolled = false,
                PaymentStatus = "NotEnrolled"
            };
        }

        return new EnrollmentStatusResult
        {
            IsEnrolled = true,
            EnrollmentDate = enrollment.EnrolledAt,
            PaymentStatus = enrollment.Status.ToString(),
            PaidAmount = enrollment.PurchasePrice,
            TotalAmount = enrollment.Course.Price,
            CanPayInstallment = enrollment.Course.Price > 1000000 // دوره‌های بالای 1 میلیون تومان قابلیت قسطی دارند
        };
    }
}