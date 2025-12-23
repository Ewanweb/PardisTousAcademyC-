using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Sliders;
using System.Text.Json.Serialization;

namespace Pardis.Application.Sliders.HeroSlides.Create
{
    public class CreateHeroSlideCommand : IRequest<OperationResult>
    {
        public CreateHeroSlideDto Dto { get; set; }

        [JsonIgnore]
        public string? CurrentUserId { get; set; }

        public CreateHeroSlideCommand(CreateHeroSlideDto dto)
        {
            Dto = dto;
        }
    }
}