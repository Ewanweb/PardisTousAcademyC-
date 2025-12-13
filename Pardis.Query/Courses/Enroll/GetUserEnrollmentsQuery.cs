using MediatR;
using Pardis.Domain.Dto.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Query.Courses.Enroll
{
    public class GetUserEnrollmentsQuery : IRequest<List<CourseResource>>
    {
        public string UserId { get; set; }
    }
}
