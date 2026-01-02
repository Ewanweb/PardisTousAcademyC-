using MediatR;
using AutoMapper;
using Pardis.Application.Shopping.Contracts;

namespace Pardis.Query.Shopping.GetMyCart;

/// <summary>
/// پردازشگر کوئری دریافت سبد خرید کاربر جاری
/// </summary>
public class GetMyCartHandler : IRequestHandler<GetMyCartQuery, GetMyCartResult?>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetMyCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<GetMyCartResult?> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart == null)
            return null;

        var result = new GetMyCartResult
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            TotalAmount = cart.TotalAmount,
            Currency = cart.Currency,
            ItemCount = cart.GetItemCount(),
            IsExpired = cart.IsExpired(),
            ExpiresAt = cart.ExpiresAt,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                CourseId = item.CourseId,
                UnitPrice = item.UnitPrice,
                Title = item.TitleSnapshot,
                Thumbnail = item.ThumbnailSnapshot,
                Instructor = item.InstructorSnapshot,
                AddedAt = item.CreatedAt
            }).ToList()
        };

        return result;
    }
}