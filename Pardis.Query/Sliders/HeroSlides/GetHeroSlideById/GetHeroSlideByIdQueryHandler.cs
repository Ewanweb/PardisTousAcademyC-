using AutoMapper;
using MediatR;
using Pardis.Domain.Dto.Sliders;
using Pardis.Domain.Sliders;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlideById
{
    public class GetHeroSlideByIdQueryHandler : IRequestHandler<GetHeroSlideByIdQuery, HeroSlideResource?>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly IMapper _mapper;

        public GetHeroSlideByIdQueryHandler(IHeroSlideRepository heroSlideRepository, IMapper mapper)
        {
            _heroSlideRepository = heroSlideRepository;
            _mapper = mapper;
        }

        public async Task<HeroSlideResource?> Handle(GetHeroSlideByIdQuery request, CancellationToken cancellationToken)
        {
            var heroSlide = await _heroSlideRepository.GetHeroSlideByIdAsync(request.Id, cancellationToken);

            if (heroSlide == null)
                return null;

            return _mapper.Map<HeroSlideResource>(heroSlide);
        }
    }
}