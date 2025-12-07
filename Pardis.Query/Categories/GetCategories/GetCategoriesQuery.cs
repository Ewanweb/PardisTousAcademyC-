using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis.Domain.Dto.Categories;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Categories.GetCategories
{
    public class GetCategoriesQuery : IRequest<List<CategoryResource>>;
}
