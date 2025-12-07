namespace Pardis.Domain.Dto.Users;

public class AuthResultDto
{
    public string Token { get; set; }
    public UserResource User { get; set; }
}