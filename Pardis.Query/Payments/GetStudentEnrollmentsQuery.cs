using MediatR;
using Pardis.Domain.Dto.Payments;

namespace Pardis.Query.Payments;

/// <summary>
/// Query دریافت اقساط دانشجو
/// </summary>
public class GetStudentEnrollmentsQuery : IRequest<List<CourseEnrollmentDto>>
{
    public string StudentId { get; set; } = string.Empty;
}