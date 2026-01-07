using MediatR;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Query.Payments.GetStudentEnrollment;

/// <summary>
/// Query برای دریافت جزئیات یک ثبت‌نام خاص برای دانشجو
/// </summary>
public class GetStudentEnrollmentDetailsQuery : IRequest<CourseEnrollmentDto>
{
    public Guid EnrollmentId { get; set; }
    public string StudentId { get; set; } = string.Empty;
}
