using MediatR;
using Pardis.Domain.Dto.Sliders;

namespace Pardis.Query.Sliders.SuccessStories.GetSuccessStoryById
{
    public class GetSuccessStoryByIdQuery : IRequest<SuccessStoryResource?>
    {
        public Guid Id { get; set; }

        public GetSuccessStoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}