//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Api.Areas.Admin.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CategoryController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public CategoryController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        // لیست دسته‌بندی‌ها (معادل index)
//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            var query = new GetCategoriesQuery { IncludeChildren = true, IncludeSeo = true };
//            var result = await _mediator.Send(query);
//            return Ok(new { data = result });
//        }

//        // ایجاد دسته‌بندی (معادل store)
//        [HttpPost]
//        [Authorize(Roles = "Admin,Manager")]
//        public async Task<IActionResult> Store([FromForm] CreateCategoryCommand command)
//        {
//            // پر کردن خودکار UserId از توکن
//            command.CurrentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//            var result = await _mediator.Send(command);
//            return StatusCode(201, new { message = "دسته‌بندی با موفقیت ایجاد شد.", data = result });
//        }

//        // نمایش تکی (معادل show)
//        [HttpGet("{id}")]
//        public async Task<IActionResult> Show(Guid id)
//        {
//            var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
//            return Ok(new { message = "دسته‌بندی نمایش داده شد.", data = result });
//        }

//        // نمایش زیرمجموعه‌ها (معادل children)
//        [HttpGet("{id}/children")]
//        public async Task<IActionResult> Children(Guid id)
//        {
//            // این کوئری باید لیست فرزندان + تعداد دوره‌ها را برگرداند
//            var result = await _mediator.Send(new GetCategoryChildrenQuery { ParentId = id });

//            return Ok(new
//            {
//                message = $"زیرمجموعه‌های دسته دریافت شد.",
//                parent = result.ParentInfo, // اطلاعات پدر
//                data = result.Children // لیست فرزندان
//            });
//        }

//        // ویرایش (معادل update)
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Admin,Manager")]
//        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCategoryCommand command)
//        {
//            command.Id = id;
//            var result = await _mediator.Send(command);
//            return Ok(new { message = "دسته‌بندی ویرایش شد.", data = result });
//        }

//        // حذف (معادل destroy با قابلیت انتقال محتوا)
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Destroy(Guid id, [FromBody] DeleteCategoryDto requestBody)
//        {
//            try
//            {
//                var command = new DeleteCategoryCommand
//                {
//                    Id = id,
//                    MigrateToId = requestBody?.MigrateToId // نال‌پذیر
//                };

//                await _mediator.Send(command);

//                var msg = "دسته‌بندی با موفقیت حذف شد" + (command.MigrateToId.HasValue ? " و محتوا منتقل گردید." : ".");
//                return Ok(new { message = msg });
//            }
//            catch (Exception ex)
//            {
//                return Conflict(new { message = ex.Message, error = "CONTENT_DEPENDENCY_ERROR" });
//            }
//        }
//    }

//    // کلاس کمکی برای بادیِ حذف
//    public class DeleteCategoryDto { public Guid? MigrateToId { get; set; } }
//}
//}
