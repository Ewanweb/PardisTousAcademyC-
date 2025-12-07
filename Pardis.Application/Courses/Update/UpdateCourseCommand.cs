using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Domain.Courses;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Pardis.Domain.Dto.Courses;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Courses.Update
{
    public class UpdateCourseCommand() : IRequest<OperationResult<CourseResource>>
    {
        public UpdateCourseDto Dto { get; set; }
    }

}