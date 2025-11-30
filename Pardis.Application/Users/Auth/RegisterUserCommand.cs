using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Application._Shared;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application.Users.Auth
{
    public class RegisterUserCommand : IRequest<OperationResult<AuthResultDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
    }

}
