using System.Collections.Generic;
using MediatR;
using Pardis.Domain.Dto.Sliders;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlides
{
    public class GetHeroSlidesQuery : IRequest<List<HeroSlideListResource>>
    {
        public bool IncludeInactive { get; set; } = false;
        public bool IncludeExpired { get; set; } = false;
        public bool AdminView { get; set; } = false;
    }
}