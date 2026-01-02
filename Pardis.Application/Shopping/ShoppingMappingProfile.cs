using AutoMapper;
using Pardis.Domain.Shopping;
// Temporarily commented out to fix build issues
// using Pardis.Application.Shopping.Cart.AddCourseToCart;
// using Pardis.Application.Shopping.Cart.RemoveCourseFromCart;
// using Pardis.Application.Shopping.Cart.ClearCart;
// using Pardis.Application.Shopping.Checkout.CreateCheckout;
// using Pardis.Application.Shopping.PaymentAttempts.UploadReceipt;
// using Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;
// using Pardis.Query.Shopping.GetMyCart;
// using Pardis.Query.Shopping.GetMyOrders;
// using Pardis.Query.Shopping.GetPendingPayments;

namespace Pardis.Application.Shopping;

/// <summary>
/// پروفایل نگاشت AutoMapper برای موجودیت‌های سبد خرید
/// </summary>
public class ShoppingMappingProfile : Profile
{
    public ShoppingMappingProfile()
    {
        // Temporarily commented out to fix build issues
        // Cart mappings will be implemented when shopping queries are available
    }
}