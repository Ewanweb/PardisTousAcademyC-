using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardis.Application._Shared;
using Pardis.Application.Categories.Create;
using Pardis.Application.Categories.Delete;
using Pardis.Application.Categories.Update;
using Pardis.Domain.Users;
using Pardis.Query.Categories.GetCategories;
using Pardis.Query.Categories.GetCategoryById;
using Pardis.Query.Categories.GetCategoryChildren;
using System.Security.Claims;

namespace Api.Areas.Admin.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = "Admin,Manager")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // لیست دسته‌بندی‌ها
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return Ok(new { data = result });
        }

        // نمایش تکی
        [HttpGet("{id}")]
        public async Task<IActionResult> Show(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            return Ok(new { data = result });
        }

        // نمایش زیرمجموعه‌ها
        [HttpGet("{id}/children")]
        public async Task<IActionResult> Children(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryChildrenQuery { ParentId = id });
            return Ok(result); // ساختار بازگشتی شامل Parent و Children
        }

        // ایجاد دسته‌بندی
        [HttpPost]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        public async Task<IActionResult> Store([FromBody] CreateCategoryCommand command)
        {
            command.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(command);

            return StatusCode(201, new { message = "دسته‌بندی با موفقیت ایجاد شد." });
        }

        // ویرایش
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (result.Status == OperationResultStatus.NotFound) return NotFound(result.Message);

            return Ok(new { message = "دسته‌بندی با موفقیت ویرایش شد." });
        }

        // حذف (با قابلیت انتقال محتوا)
        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Destroy(Guid id, [FromQuery] Guid? migrate_to_id)
        {
            // نکته: در PHP پارامتر را از بادی می‌گرفتید، اینجا از کوئری استرینگ گرفتیم که استانداردتر است.
            // اگر از بادی می‌خواهید، یک DTO بسازید و [FromBody] بگیرید.

            var result = await _mediator.Send(new DeleteCategoryCommand { Id = id, MigrateToId = migrate_to_id });

            if (result.Status == OperationResultStatus.Error)
                return Conflict(new { message = result.Message, error = "CONTENT_DEPENDENCY_ERROR" });

            if (result.Status == OperationResultStatus.NotFound)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}

