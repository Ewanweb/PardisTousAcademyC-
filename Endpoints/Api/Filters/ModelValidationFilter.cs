using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

/// <summary>
/// فیلتر اعتبارسنجی مدل‌ها
/// </summary>
public class ModelValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new
            {
                success = false,
                message = "اطلاعات ارسالی نامعتبر است",
                errors = errors.Any() ? errors : new List<string> { "خطا در اعتبارسنجی داده‌ها" }
            };

            context.Result = new BadRequestObjectResult(response);
        }

        base.OnActionExecuting(context);
    }
}