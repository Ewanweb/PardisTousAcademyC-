using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Consultation;

namespace Api.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/consultations")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ConsultationAdminController : BaseController
{
    public ConsultationAdminController(IMediator mediator, ILogger<ConsultationAdminController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// دریافت لیست درخواست‌های مشاوره
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConsultationRequests([FromQuery] GetConsultationRequestsQuery query)
    {
        try
        {
            var result = await Mediator.Send(query);
            return HandleOperationResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت درخواست‌های مشاوره");
            return ErrorResponse("خطا در دریافت درخواست‌های مشاوره", 500);
        }
    }

    /// <summary>
    /// به‌روزرسانی وضعیت درخواست مشاوره
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateConsultationStatusCommand command)
    {
        try
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return HandleOperationResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی وضعیت درخواست مشاوره");
            return ErrorResponse("خطا در به‌روزرسانی وضعیت", 500);
        }
    }

    /// <summary>
    /// حذف درخواست مشاوره
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsultationRequest(Guid id)
    {
        try
        {
            var repository = HttpContext.RequestServices.GetRequiredService<Pardis.Domain.Consultation.IConsultationRequestRepository>();
            await repository.DeleteAsync(id);
            return SuccessResponse("درخواست مشاوره با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف درخواست مشاوره");
            return ErrorResponse("خطا در حذف درخواست مشاوره", 500);
        }
    }
}
