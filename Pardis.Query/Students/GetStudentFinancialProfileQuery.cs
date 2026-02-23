using MediatR;
using Pardis.Domain.Dto.Students;

namespace Pardis.Query.Students;

/// <summary>
/// Query برای دریافت پروفایل مالی دانشجو
/// </summary>
public class GetStudentFinancialProfileQuery : IRequest<StudentFinancialProfileDto>
{
    public string StudentId { get; set; } = string.Empty;
}
