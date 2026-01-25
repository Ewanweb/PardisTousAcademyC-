using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.Cart.AddCourseToCart;

/// <summary>
/// دستور اضافه کردن دوره به سبد خرید
/// </summary>
public class AddCourseToCartCommand : IRequest<OperationResult<AddCourseToCartResult>>
{
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
}