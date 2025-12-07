using MediatR;
using Pardis.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis.Domain.Dto.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCoursesBySlug
{
    public record GetCoursesBySlugQuery(string Slug) : IRequest<CourseResource>;
}
