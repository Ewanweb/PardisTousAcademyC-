using MediatR;
using Pardis.Domain.Dto.Students;

namespace Pardis.Query.Students;

/// <summary>
/// Query دریافت پروفایل کامل دانشجو
/// </summary>
public class GetStudentProfileQuery : IRequest<StudentProfileDto?>
{
    public string StudentId { get; set; } = string.Empty;
}