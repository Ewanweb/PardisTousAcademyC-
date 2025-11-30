using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Courses;

public partial class SoftDeleteCommandHandler
{
    public class RestoreCourseCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
    }
}