using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Sliders;
using System.Text.Json.Serialization;

namespace Pardis.Application.Sliders.SuccessStories.Update
{
    public class UpdateSuccessStoryCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
        public UpdateSuccessStoryDto Dto { get; set; }

        [JsonIgnore]
        public string? CurrentUserId { get; set; }

        public UpdateSuccessStoryCommand(Guid id, UpdateSuccessStoryDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}