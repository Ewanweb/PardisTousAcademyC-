using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto;
using Pardis.Domain.Users;

namespace Pardis.Application.Courses.Create;

public record CreateCourseCommand(Dtos.CreateCourseDto Dto, User CurrentUser, bool IsAdmin) : IRequest<OperationResult>;