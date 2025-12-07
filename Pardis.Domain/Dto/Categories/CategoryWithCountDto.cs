using Pardis.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Domain.Dto.Categories
{
    public class CategoryWithCountDto
    {
        public Category Category { get; set; }
        public int CoursesCount { get; set; }
    }
}
