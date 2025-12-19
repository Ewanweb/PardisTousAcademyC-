using MediatR;
using Pardis.Domain.Dto.Students;

namespace Pardis.Query.Students;

/// <summary>
/// Query دریافت خلاصه مالی دانشجو
/// </summary>
public class GetStudentFinancialSummaryQuery : IRequest<StudentFinancialSummaryDto?>
{
    public string StudentId { get; set; } = string.Empty;
}