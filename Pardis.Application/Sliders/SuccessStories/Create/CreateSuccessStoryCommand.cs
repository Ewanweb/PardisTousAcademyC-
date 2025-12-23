using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Sliders;
using System.Text.Json.Serialization;

namespace Pardis.Application.Sliders.SuccessStories.Create
{
    public class CreateSuccessStoryCommand : IRequest<OperationResult>
    {
        public CreateSuccessStoryDto Dto { get; set; }

        [JsonIgnore]
        public string? CurrentUserId { get; set; }

        public CreateSuccessStoryCommand(CreateSuccessStoryDto dto)
        {
            Dto = dto;
        }
    }
}