using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Consultation;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ConsultationController : BaseController
{
    public ConsultationController(IMediator mediator, ILogger<ConsultationController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// ثبت درخواست مشاوره
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateConsultationRequest([FromBody] CreateConsultationRequestCommand command)
    {
        try
        {
            // Get user ID if authenticated
            var userId = GetCurrentUserId();
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
            {
                command.UserId = userGuid;
            }

            var result = await Mediator.Send(command);
            return HandleOperationResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ثبت درخواست مشاوره");
            return ErrorResponse("خطا در ثبت درخواست مشاوره", 500);
        }
    }
}
