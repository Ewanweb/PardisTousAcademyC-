using MediatR;
using AutoMapper;
using Pardis.Application._Shared;
using Pardis.Application.Shopping.Contracts;

namespace Pardis.Application.Shopping.Cart.RemoveCourseFromCart;

/// <summary>
/// پردازشگر دستور حذف دوره از سبد خرید
/// </summary>
public class RemoveCourseFromCartHandler : IRequestHandler<RemoveCourseFromCartCommand, OperationResult<RemoveCourseFromCartResult>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public RemoveCourseFromCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<OperationResult<RemoveCourseFromCartResult>> Handle(RemoveCourseFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // دریافت سبد خرید کاربر
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
                return OperationResult<RemoveCourseFromCartResult>.NotFound("سبد خرید یافت نشد");

            // بررسی وجود دوره در سبد
            if (!cart.ContainsCourse(request.CourseId))
                return OperationResult<RemoveCourseFromCartResult>.NotFound("دوره در سبد خرید یافت نشد");

            // حذف دوره از سبد
            cart.RemoveCourse(request.CourseId);
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            // ایجاد نتیجه
            var result = new RemoveCourseFromCartResult
            {
                CartId = cart.Id,
                TotalItems = cart.GetItemCount(),
                TotalAmount = cart.TotalAmount,
                Message = "دوره با موفقیت از سبد خرید حذف شد"
            };

            return OperationResult<RemoveCourseFromCartResult>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<RemoveCourseFromCartResult>.Error($"خطا در حذف دوره از سبد خرید: {ex.Message}");
        }
    }
}