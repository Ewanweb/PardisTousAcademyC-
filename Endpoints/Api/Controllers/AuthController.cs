using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Users.Auth;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Status != OperationResultStatus.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return CreatedAtAction(nameof(Login), new { message = "ثبت‌نام با موفقیت انجام شد.", data = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            // لاگین و تولید توکن در هندلر انجام می‌شود
            var result = await _mediator.Send(command);

            if (result.Status != OperationResultStatus.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(new
            {
                message = "ورود موفقیت‌آمیز بود.",
                data = result
            });
        }
    }
}

