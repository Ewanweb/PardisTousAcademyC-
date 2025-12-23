using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            if (!request.IncludeExpired)
            {
                query = query.Where(x => x.IsPermanent || (x.ExpiresAt.HasValue && x.ExpiresAt.Value > DateTime.UtcNow));
            }

            var heroSlides = await query
                .OrderBy(x => x.Order)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = heroSlides.Select(slide => new HeroSlideListResource
            {
                Id = slide.Id,
                Title = slide.Title,
                ImageUrl = slide.ImageUrl,
                LinkUrl = slide.LinkUrl,
                ButtonText = slide.ButtonText,
                Order = slide.Order,
                IsActive = slide.IsActive,
                IsPermanent = slide.IsPermanent,
                ExpiresAt = slide.ExpiresAt,
                TimeRemaining = slide.GetTimeRemaining(),
                IsExpired = slide.IsExpired()
            }).ToList();

            return result;
        }
    }
}