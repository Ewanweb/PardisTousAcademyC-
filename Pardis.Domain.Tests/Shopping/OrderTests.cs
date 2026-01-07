using Pardis.Domain.Shopping;
using Xunit;

namespace Pardis.Domain.Tests.Shopping;

public class OrderTests
{
    [Fact]
    public void Order_Constructor_WithValidCart_SetsCartIdCorrectly()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = CreateValidCart();

        // Act
        var order = new Order(userId, cart);

        // Assert
        Assert.Equal(cart.Id, order.CartId);
        Assert.NotEqual(Guid.Empty, order.CartId);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(OrderStatus.Draft, order.Status);
    }

    [Fact]
    public void Order_Constructor_WithEmptyCartId_ThrowsException()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = CreateCartWithEmptyId();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => new Order(userId, cart));
        Assert.Contains("شناسه سبد خرید نمی‌تواند خالی باشد", exception.Message);
    }

    [Fact]
    public void Order_Constructor_WithNullCart_ThrowsException()
    {
        // Arrange
        var userId = "test-user-id";
        Cart cart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Order(userId, cart));
    }

    [Fact]
    public void Order_Constructor_WithEmptyUserId_ThrowsException()
    {
        // Arrange
        var userId = "";
        var cart = CreateValidCart();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Order(userId, cart));
        Assert.Contains("شناسه کاربر نمی‌تواند خالی باشد", exception.Message);
    }

    [Fact]
    public void Order_CreatePaymentAttempt_SetsOrderToPendingPayment()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act
        var paymentAttempt = order.CreatePaymentAttempt(100000);

        // Assert
        Assert.Equal(OrderStatus.PendingPayment, order.Status);
        Assert.Single(order.PaymentAttempts);
        Assert.Equal(order.Id, paymentAttempt.OrderId);
    }

    [Fact]
    public void Order_CompleteOrder_WithSuccessfulPayment_CompletesSuccessfully()
    {
        // Arrange
        var order = CreateValidOrder();
        var paymentAttempt = order.CreatePaymentAttempt(100000);
        paymentAttempt.ApproveByAdmin("admin-user-id");

        // Act
        order.CompleteOrder();

        // Assert
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.NotNull(order.CompletedAt);
    }

    private Cart CreateValidCart()
    {
        var cart = new Cart("test-user-id");
        // Simulate cart being saved to DB (which would assign an ID)
        var cartIdField = typeof(Cart).GetField("<Id>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cartIdField?.SetValue(cart, Guid.NewGuid());
        
        // Add a mock course to make cart non-empty
        var courseId = Guid.NewGuid();
        // We would need to create a proper Course entity here in a real test
        // For now, we'll simulate a non-empty cart by setting TotalAmount
        var totalAmountField = typeof(Cart).GetField("<TotalAmount>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        totalAmountField?.SetValue(cart, 100000L);
        
        return cart;
    }

    private Cart CreateCartWithEmptyId()
    {
        var cart = new Cart("test-user-id");
        // Don't set ID, leaving it as Guid.Empty
        var totalAmountField = typeof(Cart).GetField("<TotalAmount>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        totalAmountField?.SetValue(cart, 100000L);
        
        return cart;
    }

    private Order CreateValidOrder()
    {
        var cart = CreateValidCart();
        return new Order("test-user-id", cart);
    }
}