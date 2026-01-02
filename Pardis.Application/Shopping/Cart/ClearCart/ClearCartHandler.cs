using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;

namespace Pardis.Application.Shopping.Cart.ClearCart;

/// <summary>
/// پردازشگر دستور پاک کردن سبد خرید
/// </summary>
public class ClearCartHandler : IRequestHandler<ClearCartCommand, OperationResult<ClearCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ClearCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<ClearCartResult>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت سبد خرید کاربر
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
                return OperationResult<ClearCartResult>.NotFound("سبد خرید یافت نشد");

            // پاک کردن سبد
            cart.Clear();
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new ClearCartResult
            {
                CartId = cart.Id,
                Message = "سبد خرید با موفقیت پاک شد"
            };

            return OperationResult<ClearCartResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<ClearCartResult>.Error($"خطا در پاک کردن سبد خرید: {ex.Message}");
        }
    }
}