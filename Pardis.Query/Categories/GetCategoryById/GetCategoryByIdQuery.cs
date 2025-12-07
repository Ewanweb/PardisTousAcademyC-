using MediatR;
using Pardis.Domain.Dto.Categories;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryResource>
    {
        public Guid Id { get; set; }
    }
}
