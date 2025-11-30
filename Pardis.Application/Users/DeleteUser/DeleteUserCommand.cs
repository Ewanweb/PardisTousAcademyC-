using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Query.Users.GetUsers;

public partial class CreateUserByAdminHandler
{
    public class DeleteUserCommand : IRequest<OperationResult>
    {
        public string Id { get; set; }
    }
}