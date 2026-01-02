using MediatR;
using Pardis.Application._Shared;

namespace Pardis.Application.Shopping.Cart.ClearCart;

/// <summary>
/// دستور پاک کردن سبد خرید
/// </summary>
public class ClearCartCommand : IRequest<OperationResult<ClearCartResult>>
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه پاک کردن سبد خرید
/// </summary>
public class ClearCartResult
{
    public Guid CartId { get; set; }
    public string Message { get; set; } = string.Empty;
}