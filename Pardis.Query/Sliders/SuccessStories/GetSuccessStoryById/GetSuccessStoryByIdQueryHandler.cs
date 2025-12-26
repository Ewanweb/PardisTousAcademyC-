using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Sliders;
using Pardis.Domain.Sliders;

namespace Pardis.Query.Sliders.SuccessStories.GetSuccessStoryById
{
    public class GetSuccessStoryByIdQueryHandler : IRequestHandler<GetSuccessStoryByIdQuery, SuccessStoryResource?>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly IMapper _mapper;

        public GetSuccessStoryByIdQueryHandler(ISuccessStoryRepository successStoryRepository, IMapper mapper)
        {
            _successStoryRepository = successStoryRepository;
            _mapper = mapper;
        }

        public async Task<SuccessStoryResource?> Handle(GetSuccessStoryByIdQuery request, CancellationToken cancellationToken)
        {
            var successStory = await _successStoryRepository.GetSuccessStoryByIdAsync(request.Id, cancellationToken);

            if (successStory == null)
                return null;

            return _mapper.Map<SuccessStoryResource>(successStory);
        }
    }
}