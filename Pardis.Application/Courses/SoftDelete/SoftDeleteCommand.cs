using MediatR;

namespace Pardis.Application.Courses;

public record SoftDeleteCommand(Guid Id) : IRequest<OperationResult>;