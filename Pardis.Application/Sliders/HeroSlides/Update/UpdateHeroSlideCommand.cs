using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Sliders;
using System.Text.Json.Serialization;

namespace Pardis.Application.Sliders.HeroSlides.Update
{
    public class UpdateHeroSlideCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
        public UpdateHeroSlideDto Dto { get; set; }

        [JsonIgnore]
        public string? CurrentUserId { get; set; }

        public UpdateHeroSlideCommand(Guid id, UpdateHeroSlideDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}