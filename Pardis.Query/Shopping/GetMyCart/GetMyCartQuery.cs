using MediatR;

namespace Pardis.Query.Shopping.GetMyCart;

/// <summary>
/// کوئری دریافت سبد خرید کاربر جاری
/// </summary>
public class GetMyCartQuery : IRequest<GetMyCartResult?>
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه دریافت سبد خرید
/// </summary>
public class GetMyCartResult
{
    public Guid CartId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long TotalAmount { get; set; }
    public string Currency { get; set; } = "IRR";
    public int ItemCount { get; set; }
    public bool IsExpired { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO آیتم سبد خرید
/// </summary>
public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public long UnitPrice { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string? Instructor { get; set; }
    public DateTime AddedAt { get; set; }
}