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
            List<HeroSlide> heroSlides;
            
            if (request.IncludeInactive)
            {
                // Get all slides (active and inactive) ordered by Order then CreatedAt
                var allSlides = await _heroSlideRepository.GetAllAsync();
                heroSlides = allSlides.OrderBy(x => x.Order).ThenBy(x => x.CreatedAt).ToList();
            }
            else
            {
                // Get only active slides
                heroSlides = await _heroSlideRepository.GetActiveHeroSlidesAsync(cancellationToken);
            }

            return _mapper.Map<List<HeroSlideListResource>>(heroSlides);
        }
    }
}