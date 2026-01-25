using Pardis.Domain.Service;

namespace Api.Service;

public class RequestContext :  IRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string Ip()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string UserAgent()
    {
        return _httpContextAccessor.HttpContext.Request.Headers.UserAgent;
    }

    public string? GetPlatform()
    {
        var ctx = _httpContextAccessor.HttpContext;

        if (ctx == null) return null;

        if (ctx.Request.Headers.TryGetValue("sec-ch-ua-platform", out var platform))
            return platform.ToString();

        return null;
    }
}