using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;
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
        var course = CreateMockCourse();
        
        // Temporarily bypass the ID check for testing
        var items = new List<CartItem>();
        var cartItem = new CartItem(cart.Id, course.Id, course.Price, course.Title);
        items.Add(cartItem);
        
        var itemsField = typeof(Cart).GetField("<Items>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        itemsField?.SetValue(cart, items);
        
        // Set TotalAmount
        var totalAmountField = typeof(Cart).GetField("<TotalAmount>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        totalAmountField?.SetValue(cart, course.Price);
        
        return cart;
    }

    private Course CreateMockCourse()
    {
        // Create a mock course using reflection
        var course = (Course)Activator.CreateInstance(typeof(Course), true)!;
        
        var idField = typeof(Course).GetField("<Id>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(course, Guid.NewGuid());
        
        var titleField = typeof(Course).GetField("<Title>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        titleField?.SetValue(course, "Test Course");
        
        var priceField = typeof(Course).GetField("<Price>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        priceField?.SetValue(course, 100000L);
        
        return course;
    }

    private Cart CreateCartWithEmptyId()
    {
        var cart = new Cart("test-user-id");
        // Don't set ID, leaving it as Guid.Empty
        
        // Add a mock course to make cart non-empty
        var course = CreateMockCourse();
        
        // Create items list but don't add to cart (since ID is empty)
        var items = new List<CartItem>();
        var cartItem = new CartItem(Guid.NewGuid(), course.Id, course.Price, course.Title); // Use random ID for item
        items.Add(cartItem);
        
        var itemsField = typeof(Cart).GetField("<Items>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        itemsField?.SetValue(cart, items);
        
        var totalAmountField = typeof(Cart).GetField("<TotalAmount>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        totalAmountField?.SetValue(cart, course.Price);
        
        return cart;
    }

    private Order CreateValidOrder()
    {
        var cart = CreateValidCart();
        return new Order("test-user-id", cart);
    }
}