using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses.SoftDelete;

public record SoftDeleteCommand(Guid Id) : IRequest<OperationResult>;