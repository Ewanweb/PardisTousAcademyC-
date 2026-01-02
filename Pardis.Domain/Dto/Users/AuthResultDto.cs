namespace Pardis.Domain.Dto.Users;

public class AuthResultDto
{
    public required string Token { get; set; }
    public required UserResource User { get; set; }
}