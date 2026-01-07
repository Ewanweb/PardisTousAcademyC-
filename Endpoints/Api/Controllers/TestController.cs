using Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// کنترلر تست برای بررسی authentication و authorization
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Tags("Test")]
public class TestController : BaseController
{
    public TestController(ILogger<TestController> logger) : base(logger)
    {
    }

    /// <summary>
    /// تست endpoint عمومی (بدون نیاز به authentication)
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicTest()
    {
        return Ok(new
        {
            message = "این endpoint عمومی است و نیاز به authentication ندارد",
            timestamp = DateTime.UtcNow,
            success = true
        });
    }

    /// <summary>
    /// تست endpoint محافظت شده (نیاز به authentication)
    /// </summary>
    [HttpGet("protected")]
    [Authorize]
    public IActionResult ProtectedTest()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            message = "شما با موفقیت احراز هویت شده‌اید!",
            user = new
            {
                id = userId,
                email = userEmail,
                name = userName,
                roles = roles
            },
            timestamp = DateTime.UtcNow,
            success = true
        });
    }

    /// <summary>
    /// تست endpoint با policy (نیاز به نقش Admin)
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Policy = Policies.UserManagement.Access)]
    public IActionResult AdminOnlyTest()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            message = "شما دسترسی Admin دارید!",
            user = new
            {
                id = userId,
                email = userEmail,
                roles = roles
            },
            timestamp = DateTime.UtcNow,
            success = true
        });
    }

    /// <summary>
    /// تست endpoint برای بررسی token info
    /// </summary>
    [HttpGet("token-info")]
    [Authorize]
    public IActionResult TokenInfo()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new
        {
            message = "اطلاعات token شما",
            claims = claims,
            isAuthenticated = User.Identity?.IsAuthenticated,
            authenticationType = User.Identity?.AuthenticationType,
            timestamp = DateTime.UtcNow,
            success = true
        });
    }
}