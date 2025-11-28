using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Application.Courses.Create;
using Pardis.Domain.Dto;
using Pardis.Domain.Users;

namespace Pardis.Facade.Commands
{
    public class CourseFacade
    {
        private readonly IMediator _mediator;

        public CourseFacade(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<OperationResult> Create(Dtos.CreateCourseDto dto, User currentUser, bool isAdmin)
        {
            return await _mediator.Send(new CreateCourseCommand(dto, currentUser, isAdmin));
        }
    }
}
