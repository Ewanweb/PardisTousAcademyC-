using MediatR;
using Pardis.Application._Shared;
using System.Text.Json.Serialization;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Query.Users.GetUsers;

public partial class CreateUserByAdminHandler
{
    public class UpdateUserCommand : IRequest<OperationResult<UserResource>>
    {
        public string Id { get; set; } // از URL پر می‌شود

        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();

    }
}