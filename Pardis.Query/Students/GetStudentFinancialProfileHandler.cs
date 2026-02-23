using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Dto.Students;
using Pardis.Domain.Payments;
using Pardis.Domain.Users;

namespace Pardis.Query.Students;

/// <summary>
/// Handler برای دریافت پروفایل مالی دانشجو
/// </summary>
public class GetStudentFinancialProfileHandler : IRequestHandler<GetStudentFinancialProfileQuery, StudentFinancialProfileDto>
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly IRepository<User> _userRepository;

    public GetStudentFinancialProfileHandler(
        ICourseEnrollmentRepository enrollmentRepository,
        IRepository<User> userRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<StudentFinancialProfileDto> Handle(GetStudentFinancialProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت اطلاعات دانشجو
            var student = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);

            if (student == null)
            {
                // برگرداندن پروفایل خالی به جای throw کردن exception
                return new StudentFinancialProfileDto
                {
                    Student = new StudentBasicInfoDto
                    {
                        Id = request.StudentId,
                        FullName = "دانشجو یافت نشد",
                        Email = "",
                        PhoneNumber = ""
                    },
                    Enrollments = new List<EnrollmentFinancialDto>()
                };
            }

            // دریافت ثبت‌نام‌های دانشجو
            List<CourseEnrollment> enrollments;
            try
            {
                enrollments = await _enrollmentRepository.GetEnrollmentsWithInstallmentsAsync(request.StudentId, cancellationToken);
            }
            catch
            {
                // اگر خطایی در دریافت ثبت‌نام‌ها رخ داد، لیست خالی برگردان
                enrollments = new List<CourseEnrollment>();
            }

            var result = new StudentFinancialProfileDto
            {
                Student = new StudentBasicInfoDto
                {
                    Id = student.Id,
                    FullName = student.FullName ?? student.UserName ?? "نامشخص",
                    Email = student.Email ?? "",
                    PhoneNumber = student.Mobile ?? "",
                    Avatar = student.Avatar
                },
                Enrollments = enrollments
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => MapToEnrollmentDto(e))
                    .ToList()
            };

            return result;
        }
        catch (Exception ex)
        {
            // لاگ کردن خطا برای debugging
            Console.WriteLine($"Error in GetStudentFinancialProfileHandler: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            // برگرداندن پروفایل خالی به جای throw کردن exception
            return new StudentFinancialProfileDto
            {
                Student = new StudentBasicInfoDto
                {
                    Id = request.StudentId,
                    FullName = "خطا در دریافت اطلاعات",
                    Email = "",
                    PhoneNumber = ""
                },
                Enrollments = new List<EnrollmentFinancialDto>()
            };
        }
    }

    private static EnrollmentFinancialDto MapToEnrollmentDto(CourseEnrollment e)
    {
        try
        {
            return new EnrollmentFinancialDto
            {
                Id = e.Id,
                CourseId = e.CourseId,
                Course = new CourseBasicInfoDto
                {
                    Id = e.Course?.Id ?? Guid.Empty,
                    Title = e.Course?.Title ?? "دوره نامشخص",
                    Thumbnail = e.Course?.Thumbnail
                },
                TotalAmount = e.TotalAmount,
                PaidAmount = e.PaidAmount,
                PaymentStatus = e.PaymentStatus.ToString(),
                EnrollmentStatus = e.EnrollmentStatus.ToString(),
                EnrollmentDate = e.EnrollmentDate,
                IsInstallmentAllowed = e.IsInstallmentAllowed,
                Installments = e.InstallmentPayments?
                    .OrderBy(i => i.InstallmentNumber)
                    .Select(i => new InstallmentDto
                    {
                        Id = i.Id,
                        InstallmentNumber = i.InstallmentNumber,
                        Amount = i.Amount,
                        PaidAmount = i.PaidAmount,
                        DueDate = i.DueDate,
                        Status = i.Status.ToString(),
                        PaidDate = i.PaidDate
                    }).ToList() ?? new List<InstallmentDto>()
            };
        }
        catch
        {
            // اگر خطایی در mapping رخ داد، یک DTO خالی برگردان
            return new EnrollmentFinancialDto
            {
                Id = e.Id,
                CourseId = e.CourseId,
                Course = new CourseBasicInfoDto
                {
                    Id = Guid.Empty,
                    Title = "خطا در دریافت اطلاعات دوره"
                },
                TotalAmount = e.TotalAmount,
                PaidAmount = e.PaidAmount,
                PaymentStatus = e.PaymentStatus.ToString(),
                EnrollmentStatus = e.EnrollmentStatus.ToString(),
                EnrollmentDate = e.EnrollmentDate,
                IsInstallmentAllowed = false,
                Installments = new List<InstallmentDto>()
            };
        }
    }
}
