using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses;

public record SoftDeleteCommand(Guid Id) : IRequest<OperationResult>;