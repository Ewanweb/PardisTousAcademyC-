using MediatR;
using Pardis.Domain.Dto.Sliders;

namespace Pardis.Query.Sliders.SuccessStories.GetSuccessStories
{
    public class GetSuccessStoriesQuery : IRequest<List<SuccessStoryListResource>>
    {
        public bool IncludeInactive { get; set; } = false;
        public bool IncludeExpired { get; set; } = false;
        public bool AdminView { get; set; } = false;
        public string? Type { get; set; } // success, video
    }
}