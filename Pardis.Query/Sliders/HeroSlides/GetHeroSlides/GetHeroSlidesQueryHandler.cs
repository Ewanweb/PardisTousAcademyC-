using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Sliders;
using Pardis.Domain.Sliders;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlides
{
    public class GetHeroSlidesQueryHandler : IRequestHandler<GetHeroSlidesQuery, List<HeroSlideListResource>>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly IMapper _mapper;

        public GetHeroSlidesQueryHandler(IHeroSlideRepository heroSlideRepository, IMapper mapper)
        {
            _heroSlideRepository = heroSlideRepository;
            _mapper = mapper;
        }

        public async Task<List<HeroSlideListResource>> Handle(GetHeroSlidesQuery request, CancellationToken cancellationToken)
        {
            var heroSlides = await _heroSlideRepository.GetHeroSlidesWithFiltersAsync(
                includeInactive: request.IncludeInactive,
                includeExpired: request.IncludeExpired,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<HeroSlideListResource>>(heroSlides);
        }
    }
}