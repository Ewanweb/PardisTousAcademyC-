using MediatR;
using Pardis.Domain.Dto.Sliders;

namespace Pardis.Query.Sliders.SuccessStories.GetSuccessStories
{
    public class GetSuccessStoriesQuery : IRequest<List<SuccessStoryListResource>>
    {
        public bool IncludeInactive { get; set; } = false;
    }
}