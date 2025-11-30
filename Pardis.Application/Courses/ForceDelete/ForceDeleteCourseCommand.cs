using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses;

public partial class SoftDeleteCommandHandler
{
    public class ForceDeleteCourseCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
    }
}