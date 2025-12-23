using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Dto.Sliders;
using Pardis.Infrastructure;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlides
{
    public class GetHeroSlidesQueryHandler : IRequestHandler<GetHeroSlidesQuery, List<HeroSlideListResource>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetHeroSlidesQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<HeroSlideListResource>> Handle(GetHeroSlidesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.HeroSlides.AsQueryable();

            // فیلتر بر اساس وضعیت فعال/غیرفعال
            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            // فیلتر بر اساس انقضا
            if (!request.IncludeExpired)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsPermanent || !x.ExpiresAt.HasValue || x.ExpiresAt.Value > now);
            }

            // مرتب‌سازی
            query = query.OrderBy(x => x.Order).ThenByDescending(x => x.CreatedAt);

            var heroSlides = await query.ToListAsync(cancellationToken);

            return _mapper.Map<List<HeroSlideListResource>>(heroSlides);
        }
    }
}