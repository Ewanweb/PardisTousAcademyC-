using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Domain.Users;

namespace Pardis.Query.Users.GetUsers;

public partial class CreateUserByAdminHandler
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, OperationResult>
    {
        private readonly UserManager<User> _userManager;

        public DeleteUserHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken token)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
                return OperationResult.NotFound("کاربر یافت نشد.");

            // می‌توان اینجا چک کرد که آیا کاربر "Admin" اصلی است یا خیر تا حذف نشود

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return OperationResult.Error(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return OperationResult.Success("کاربر با موفقیت حذف شد.");
        }
    }
}