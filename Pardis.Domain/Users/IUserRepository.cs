namespace Pardis.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<bool> EmailIsExist(string email);
    Task<bool> MobileIsExist(string phone);
    Task<bool> UserIsExist(string userName);

}