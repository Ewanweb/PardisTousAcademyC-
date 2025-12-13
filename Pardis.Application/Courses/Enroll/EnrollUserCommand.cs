using MediatR;
using Pardis.Application._Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Application.Courses.Enroll
{
    public class EnrollUserCommand : IRequest<OperationResult<bool>>
    {
        public string UserId { get; set; }
        public Guid CourseId { get; set; }
    }
}
