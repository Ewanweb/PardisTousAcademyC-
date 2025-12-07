using MediatR;
using Pardis.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis.Domain.Dto.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Courses.GetCourses
{
    public class GetCoursesQuery : IRequest<List<CourseResource>>
    {
        public bool Trashed { get; set; }
        public Guid? CategoryId { get; set; }

        public string? CurrentUserId { get; set; }
        public bool IsAdminOrManager { get; set; }
        public bool IsInstructor { get; set; }
    }
}
