using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application.Settings.UpdateSystemSettings;
using Pardis.Domain.Users;
using Pardis.Query.Logging.GetSystemLogs;
using Pardis.Query.Settings.GetSystemSettings;

namespace Api.Controllers;

/// <summary>
/// Admin system controller - for IT managers and system administrators
/// </summary>
[Route("api/admin/system")]
[ApiController]
[Authorize(Roles = $"{Role.ITManager},{Role.Admin},{Role.Manager}")]
[Produces("application/json")]
[Tags("Admin - System")]
public class AdminSystemController : BaseController
{
    /// <summary>
    /// Constructor for admin system controller
    /// </summary>
    /// <param name="mediator">MediatR instance</param>
    /// <param name="logger">Logger instance</param>
    public AdminSystemController(IMediator mediator, ILogger<AdminSystemController> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Get system settings
    /// </summary>
    /// <returns>Current system settings with version and metadata</returns>
    /// <response code="200">Settings retrieved successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - insufficient permissions</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetSystemSettings()
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetSystemSettingsQuery();
            var result = await Mediator.Send(query);
            return SuccessResponse(result, "System settings retrieved successfully");
        }, "Error retrieving system settings");
    }

    /// <summary>
    /// Update system settings
    /// </summary>
    /// <param name="command">Settings update command with version and data</param>
    /// <returns>Updated system settings</returns>
    /// <response code="200">Settings updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - insufficient permissions</response>
    /// <response code="409">Version conflict - settings were modified by another user</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("settings")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 409)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] UpdateSystemSettingsCommand command)
    {
        return await ExecuteAsync(async () =>
        {
            if (command == null)
                return ValidationErrorResponse("Settings data is required");

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("User not authenticated");

            command.CurrentUserId = userId;

            var result = await Mediator.Send(command);
            return HandleOperationResult(result, "System settings updated successfully");
        }, "Error updating system settings");
    }

    /// <summary>
    /// Get system logs with filtering and pagination
    /// </summary>
    /// <param name="from">Start date filter (optional)</param>
    /// <param name="to">End date filter (optional)</param>
    /// <param name="level">Log level filter (optional)</param>
    /// <param name="source">Log source filter (optional)</param>
    /// <param name="eventId">Event ID filter (optional)</param>
    /// <param name="userId">User ID filter (optional)</param>
    /// <param name="requestId">Request ID filter (optional)</param>
    /// <param name="search">Search term (optional)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="sort">Sort order (default: time_desc)</param>
    /// <returns>System logs with pagination</returns>
    /// <response code="200">Logs retrieved successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - insufficient permissions</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("logs")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetSystemLogs(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? level = null,
        [FromQuery] string? source = null,
        [FromQuery] string? eventId = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? requestId = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sort = "time_desc")
    {
        return await ExecuteAsync(async () =>
        {
            var query = new GetSystemLogsQuery
            {
                From = from,
                To = to,
                Level = level,
                Source = source,
                EventId = eventId,
                UserId = userId,
                RequestId = requestId,
                Search = search,
                Page = page,
                PageSize = pageSize,
                Sort = sort
            };

            var result = await Mediator.Send(query);
            return SuccessResponse(result, "System logs retrieved successfully");
        }, "Error retrieving system logs");
    }
}