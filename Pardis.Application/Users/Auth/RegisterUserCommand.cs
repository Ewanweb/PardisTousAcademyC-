using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Application._Shared;

namespace Pardis.Application.Users.Auth
{
    public class RegisterUserCommand : IRequest<OperationResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
    }

}
