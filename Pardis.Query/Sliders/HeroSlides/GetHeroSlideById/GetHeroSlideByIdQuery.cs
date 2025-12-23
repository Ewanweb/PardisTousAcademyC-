using System;
using MediatR;
using Pardis.Domain.Dto.Sliders;

namespace Pardis.Query.Sliders.HeroSlides.GetHeroSlideById
{
    public class GetHeroSlideByIdQuery : IRequest<HeroSlideResource?>
    {
        public Guid Id { get; set; }

        public GetHeroSlideByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}