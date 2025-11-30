using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;
using Pardis.Domain.Courses;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Courses.Update
{
    // (بخش Command بدون تغییر)
    public class UpdateCourseCommand : IRequest<OperationResult<CourseResource>>
    {
        // ... فیلدها همان قبلی ...
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public long? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Description { get; set; }
        public CourseStatus Status { get; set; }
        public string? InstructorId { get; set; }
        public IFormFile? Image { get; set; }
        public SeoDto? Seo { get; set; }
        public string CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}