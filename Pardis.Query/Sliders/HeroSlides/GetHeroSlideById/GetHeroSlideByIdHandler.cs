using System.Threading;
using System.Threading.Tasks;
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

            return new HeroSlideResource
            {
                Id = heroSlide.Id,
                Title = heroSlide.Title,
                Description = heroSlide.Description,
                ImageUrl = heroSlide.ImageUrl,
                LinkUrl = heroSlide.LinkUrl,
                ButtonText = heroSlide.ButtonText,
                Order = heroSlide.Order,
                IsActive = heroSlide.IsActive,
                IsPermanent = heroSlide.IsPermanent,
                ExpiresAt = heroSlide.ExpiresAt,
                TimeRemaining = heroSlide.GetTimeRemaining(),
                IsExpired = heroSlide.IsExpired(),
                CreatedAt = heroSlide.CreatedAt,
                UpdatedAt = heroSlide.UpdatedAt,
                CreatedByUserId = heroSlide.CreatedByUserId
            };
        }
    }
}