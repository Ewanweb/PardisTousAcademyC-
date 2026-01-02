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

/// <summary>
/// نتیجه اضافه کردن دوره به سبد خرید
/// </summary>
public class AddCourseToCartResult
{
    public Guid CartId { get; set; }
    public Guid CartItemId { get; set; }
    public int TotalItems { get; set; }
    public long TotalAmount { get; set; }
    public string Message { get; set; } = string.Empty;
}