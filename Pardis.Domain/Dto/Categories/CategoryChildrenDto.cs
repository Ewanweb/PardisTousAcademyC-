using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Domain.Dto.Categories
{
    public class CategoryChildrenDto
    {
        public CategoryResource Parent { get; set; }
        public List<CategoryResource> Children { get; set; }
    }
}
