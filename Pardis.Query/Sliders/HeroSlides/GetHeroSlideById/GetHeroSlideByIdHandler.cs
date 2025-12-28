using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Sliders;
using Pardis.Infrastructure;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlideById
{
    public class GetHeroSlideByIdHandler : IRequestHandler<GetHeroSlideByIdQuery, HeroSlideResource?>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetHeroSlideByIdHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<HeroSlideResource?> Handle(GetHeroSlideByIdQuery request, CancellationToken cancellationToken)
        {
            var heroSlide = await _context.HeroSlides
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (heroSlide == null)
                return null;

            // Use AutoMapper for simplified mapping
            return _mapper.Map<HeroSlideResource>(heroSlide);
        }
    }
}