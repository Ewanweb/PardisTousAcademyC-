using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Sliders.HeroSlides.Delete
{
    public class DeleteHeroSlideCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }

        public DeleteHeroSlideCommand(Guid id)
        {
            Id = id;
        }
    }
}