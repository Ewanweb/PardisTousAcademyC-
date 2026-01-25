using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Categories.Update
{
    public class UpdateCategoryCommand : IRequest<OperationResult>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }
        public SeoDto Seo { get; set; }
    }
}
