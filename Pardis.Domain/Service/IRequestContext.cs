namespace Pardis.Domain.Service
{
    public interface IRequestContext
    {
        string Ip();
        string UserAgent();
        string? GetPlatform();
    }
}
