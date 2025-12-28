using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Sliders;
using Pardis.Infrastructure;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlides
{
    public class GetHeroSlidesHandler : IRequestHandler<GetHeroSlidesQuery, List<HeroSlideListResource>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetHeroSlidesHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<HeroSlideListResource>> Handle(GetHeroSlidesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.HeroSlides.AsQueryable();

            // Filter based on request parameters
            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            // Note: IncludeExpired parameter is kept for backward compatibility but not used
            // since the simplified slider system doesn't have expiration logic

            var heroSlides = await query
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            // Use AutoMapper for simplified mapping
            return _mapper.Map<List<HeroSlideListResource>>(heroSlides);
        }
    }
}