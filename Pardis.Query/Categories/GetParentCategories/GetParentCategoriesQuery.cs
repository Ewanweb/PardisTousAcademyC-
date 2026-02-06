using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Categories;

namespace Pardis.Query.Categories.GetParentCategories
{
    public record GetParentCategoriesQuery : IRequest<OperationResult<List<CategoryResource>>>;
}
