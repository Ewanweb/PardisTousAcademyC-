using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Sliders.SuccessStories.Delete
{
    public class DeleteSuccessStoryCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }

        public DeleteSuccessStoryCommand(Guid id)
        {
            Id = id;
        }
    }
}