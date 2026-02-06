using Api.Authorization;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
using Pardis.Query.Categories.GetParentCategories;

namespace Api.Areas.Admin.Controllers
{
    /// <summary>
    /// کنترلر مدیریت دسته‌بندی‌ها - پنل ادمین
    /// </summary>
    [Route("api/categories")]
    [ApiController]
    [Authorize]
    [Authorize(Policy = Policies.CategoryManagement.Access)]
    [Produces("application/json")]
    [Tags("Categories Management")]
    public class CategoryController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// سازنده کنترلر مدیریت دسته‌بندی‌ها
        /// </summary>
        /// <param name="mediator">واسط MediatR</param>
        /// <param name="logger">لاگر</param>
        public CategoryController(IMediator mediator, ILogger<CategoryController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام دسته‌بندی‌ها
        /// </summary>
        /// <returns>لیست دسته‌بندی‌ها</returns>
        /// <response code="200">لیست دسته‌بندی‌ها با موفقیت دریافت شد</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Index()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetCategoriesQuery());
                return SuccessResponse(result, "لیست دسته‌بندی‌ها با موفقیت دریافت شد");
            }, "خطا در دریافت لیست دسته‌بندی‌ها");
        }

        /// <summary>
        /// دریافت جزئیات یک دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        /// <returns>جزئیات دسته‌بندی</returns>
        /// <response code="200">دسته‌بندی با موفقیت دریافت شد</response>
        /// <response code="404">دسته‌بندی یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Show(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
                
                if (result == null)
                    return NotFoundResponse("دسته‌بندی یافت نشد");

                return SuccessResponse(result, "دسته‌بندی با موفقیت دریافت شد");
            }, "خطا در دریافت دسته‌بندی");
        }

        /// <summary>
        /// دریافت زیرمجموعه‌های یک دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی والد</param>
        /// <returns>لیست زیرمجموعه‌ها</returns>
        /// <response code="200">زیرمجموعه‌ها با موفقیت دریافت شد</response>
        /// <response code="404">دسته‌بندی یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpGet("{id}/children")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Children(Guid id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetCategoryChildrenQuery { ParentId = id });
                return SuccessResponse(result, "زیرمجموعه‌ها با موفقیت دریافت شد");
            }, "خطا در دریافت زیرمجموعه‌ها");
        }

        /// <summary>
        /// ایجاد دسته‌بندی جدید
        /// </summary>
        /// <param name="command">اطلاعات دسته‌بندی جدید</param>
        /// <returns>نتیجه عملیات ایجاد</returns>
        /// <response code="201">دسته‌بندی با موفقیت ایجاد شد</response>
        /// <response code="400">اطلاعات نامعتبر</response>
        /// <response code="401">عدم احراز هویت</response>
        /// <response code="403">عدم دسترسی</response>
        /// <response code="500">خطای سرور</response>
        [HttpPost]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Store([FromBody] CreateCategoryCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                command.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _mediator.Send(command);

                return HandleOperationResult(result, "دسته‌بندی با موفقیت ایجاد شد");
            }, "خطا در ایجاد دسته‌بندی");
        }

        /// <summary>
        /// ویرایش دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        /// <param name="command">اطلاعات جدید دسته‌بندی</param>
        /// <returns>نتیجه عملیات ویرایش</returns>
        /// <response code="200">دسته‌بندی با موفقیت ویرایش شد</response>
        /// <response code="400">اطلاعات نامعتبر</response>
        /// <response code="404">دسته‌بندی یافت نشد</response>
        /// <response code="500">خطای سرور</response>
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin + "," + Role.Manager)]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            return await ExecuteAsync(async () =>
            {
                command.Id = id;
                var result = await _mediator.Send(command);

                return HandleOperationResult(result, "دسته‌بندی با موفقیت ویرایش شد");
            }, "خطا در ویرایش دسته‌بندی");
        }

        /// <summary>
        /// حذف دسته‌بندی (با قابلیت انتقال محتوا)
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        /// <param name="migrate_to_id">شناسه دسته‌بندی مقصد برای انتقال محتوا</param>
        /// <returns>نتیجه عملیات حذف</returns>
        /// <response code="200">دسته‌بندی با موفقیت حذف شد</response>
        /// <response code="404">دسته‌بندی یافت نشد</response>
        /// <response code="409">تضاد - دسته‌بندی دارای محتوا است</response>
        /// <response code="500">خطای سرور</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Destroy(Guid id, [FromQuery] Guid? migrate_to_id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new DeleteCategoryCommand { Id = id, MigrateToId = migrate_to_id });

                if (result.Status == OperationResultStatus.Error)
                    return ErrorResponse(result.Message ?? "خطا در حذف دسته‌بندی", 409, "CONTENT_DEPENDENCY_ERROR");

                return HandleOperationResult(result, "دسته‌بندی با موفقیت حذف شد");
            }, "خطا در حذف دسته‌بندی");
        }

        [HttpGet("Parent")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> GetParentList()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetParentCategoriesQuery());

                if (result.Status != OperationResultStatus.Success)
                    return ErrorResponse(result.Message ?? "خطا در دریافت دسته‌بندی", 409, "CONTENT_DEPENDENCY_ERROR");

                return HandleOperationResult(result, "دسته‌بندی با موفقیت دریافت شد");
            }, "خطا در دریافت دسته‌بندی");
        }
    }
}