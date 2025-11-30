using MediatR;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategoryChildren
{
    public class GetCategoryChildrenQuery : IRequest<CategoryChildrenDto>
    {
        public Guid ParentId { get; set; }
    }
}
