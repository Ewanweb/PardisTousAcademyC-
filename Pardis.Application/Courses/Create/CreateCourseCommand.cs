using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Courses.Create;

public record CreateCourseCommand(CreateCourseDto Dto, bool IsAdmin) : IRequest<OperationResult<CourseResource>>
{
    [System.Text.Json.Serialization.JsonIgnore]
    public string? CurrentUserId { get; init; }
}