using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.Cart.RemoveCourseFromCart;

/// <summary>
/// دستور حذف دوره از سبد خرید
/// </summary>
public class RemoveCourseFromCartCommand : IRequest<OperationResult<RemoveCourseFromCartResult>>
{
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
}

/// <summary>
/// نتیجه حذف دوره از سبد خرید
/// </summary>
public class RemoveCourseFromCartResult
{
    public Guid CartId { get; set; }
    public int TotalItems { get; set; }
    public long TotalAmount { get; set; }
    public string Message { get; set; } = string.Empty;
}