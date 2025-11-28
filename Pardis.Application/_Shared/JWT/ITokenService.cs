using Pardis.Domain.Users;

namespace Pardis.Application._Shared.JWT;

public interface ITokenService
{
    string GenerateToken(User user, IList<string> roles);
}