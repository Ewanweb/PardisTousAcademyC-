using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Sliders;
using Pardis.Domain.Sliders;

namespace Pardis.Query.Sliders.SuccessStories.GetSuccessStories
{
    public class GetSuccessStoriesQueryHandler : IRequestHandler<GetSuccessStoriesQuery, List<SuccessStoryListResource>>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly IMapper _mapper;

        public GetSuccessStoriesQueryHandler(ISuccessStoryRepository successStoryRepository, IMapper mapper)
        {
            _successStoryRepository = successStoryRepository;
            _mapper = mapper;
        }

        public async Task<List<SuccessStoryListResource>> Handle(GetSuccessStoriesQuery request, CancellationToken cancellationToken)
        {
            List<SuccessStory> successStories;
            
            if (request.IncludeInactive)
            {
                // Get all stories (active and inactive) ordered by Order then CreatedAt
                var allStories = await _successStoryRepository.GetAllAsync();
                successStories = allStories.OrderBy(x => x.Order).ThenBy(x => x.CreatedAt).ToList();
            }
            else
            {
                // Get only active stories
                successStories = await _successStoryRepository.GetActiveSuccessStoriesAsync(cancellationToken);
            }

            return _mapper.Map<List<SuccessStoryListResource>>(successStories);
        }
    }
}