using Api.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pardis.Domain.Users;
using Pardis.Query.Courses.GetCourseStudents;
using Pardis.Query.Courses.GetMyCourses;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Admin courses controller - for instructors and education staff
/// </summary>
[Route("api/admin/courses")]
[ApiController]
[Authorize(Policy = Policies.AdminCourses.Access)]
[Produces("application/json")]
[Tags("Admin - Courses")]
public class AdminCoursesController : BaseController
{
    /// <summary>
    /// Constructor for admin courses controller
    /// </summary>
    /// <param name="mediator">MediatR instance</param>
    /// <param name="logger">Logger instance</param>
    public AdminCoursesController(IMediator mediator, ILogger<AdminCoursesController> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    /// Get instructor's courses
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="search">Search term (optional)</param>
    /// <param name="status">Course status filter (optional)</param>
    /// <param name="sort">Sort order (optional)</param>
    /// <returns>List of instructor's courses with pagination</returns>
    /// <response code="200">Courses retrieved successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - insufficient permissions</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("my-courses")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetMyCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? sort = null)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("User not authenticated");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? Role.User;

            var query = new GetMyCoursesQuery
            {
                UserId = userId,
                UserRole = userRole,
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                Sort = sort
            };

            var result = await Mediator.Send(query);
            return SuccessResponse(result, "Courses retrieved successfully");
        }, "Error retrieving courses");
    }

    /// <summary>
    /// Get students enrolled in a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="search">Search term (optional)</param>
    /// <param name="status">Enrollment status filter (optional)</param>
    /// <param name="from">Start date filter (optional)</param>
    /// <param name="to">End date filter (optional)</param>
    /// <param name="sort">Sort order (optional)</param>
    /// <returns>List of course students with statistics and pagination</returns>
    /// <response code="200">Students retrieved successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Forbidden - insufficient permissions</response>
    /// <response code="404">Course not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{courseId}/students")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetCourseStudents(
        Guid courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? sort = null)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedResponse("User not authenticated");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? Role.User;

            var query = new GetCourseStudentsQuery
            {
                CourseId = courseId,
                UserId = userId,
                UserRole = userRole,
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                From = from,
                To = to,
                Sort = sort
            };

            var result = await Mediator.Send(query);
            return SuccessResponse(result, "Course students retrieved successfully");
        }, "Error retrieving course students");
    }
}