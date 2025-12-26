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
            var successStories = await _successStoryRepository.GetSuccessStoriesWithFiltersAsync(
                includeInactive: request.IncludeInactive,
                includeExpired: request.IncludeExpired,
                type: request.Type,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<SuccessStoryListResource>>(successStories);
        }
    }
}