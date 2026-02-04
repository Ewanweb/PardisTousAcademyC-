using Pardis.Domain.Dto.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Domain.Dto.Categories
{
    public class CategoryResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; } // ????? Name ?? Title ???? ??????? ?? ?????
        public string Slug { get; set; }
        public string? Image { get; set; }
        public Guid? ParentId { get; set; }
        public int CoursesCount { get; set; }
        public string Creator { get; set; } // ??? ??????
        public SeoDto Seo { get; set; }
        public bool IsActive { get; set; }

    }
}


