using Pardis.Domain.Shopping;
using Xunit;

namespace Pardis.Domain.Tests.Shopping;

public class CartItemTests
{
    [Fact]
    public void CartItem_Constructor_WithValidData_InitializesCorrectly()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var unitPrice = 100000L;
        var titleSnapshot = "Test Course Title";
        var thumbnailSnapshot = "https://example.com/thumbnail.jpg";
        var instructorSnapshot = "Test Instructor";

        // Act
        var cartItem = new CartItem(cartId, courseId, unitPrice, titleSnapshot, thumbnailSnapshot, instructorSnapshot);

        // Assert
        Assert.Equal(cartId, cartItem.CartId);
        Assert.Equal(courseId, cartItem.CourseId);
        Assert.Equal(unitPrice, cartItem.UnitPrice);
        Assert.Equal(titleSnapshot, cartItem.TitleSnapshot);
        Assert.Equal(thumbnailSnapshot, cartItem.ThumbnailSnapshot);
        Assert.Equal(instructorSnapshot, cartItem.InstructorSnapshot);
    }

    [Fact]
    public void CartItem_Constructor_WithNegativePrice_ThrowsException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var negativePrice = -100L;
        var titleSnapshot = "Test Course";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new CartItem(cartId, courseId, negativePrice, titleSnapshot));
        Assert.Contains("قیمت نمی‌تواند منفی باشد", exception.Message);
    }

    [Fact]
    public void CartItem_Constructor_WithEmptyTitle_ThrowsException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var unitPrice = 100000L;
        var emptyTitle = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new CartItem(cartId, courseId, unitPrice, emptyTitle));
        Assert.Contains("عنوان دوره نمی‌تواند خالی باشد", exception.Message);
    }

    [Fact]
    public void CartItem_Constructor_WithNullTitle_ThrowsException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var unitPrice = 100000L;
        string nullTitle = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new CartItem(cartId, courseId, unitPrice, nullTitle));
        Assert.Contains("عنوان دوره نمی‌تواند خالی باشد", exception.Message);
    }

    [Fact]
    public void CartItem_Constructor_WithZeroPrice_IsValid()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var zeroPrice = 0L;
        var titleSnapshot = "Free Course";

        // Act
        var cartItem = new CartItem(cartId, courseId, zeroPrice, titleSnapshot);

        // Assert
        Assert.Equal(zeroPrice, cartItem.UnitPrice);
        Assert.Equal(titleSnapshot, cartItem.TitleSnapshot);
    }

    [Fact]
    public void CartItem_Constructor_WithOptionalParameters_HandlesNulls()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var unitPrice = 100000L;
        var titleSnapshot = "Test Course";

        // Act
        var cartItem = new CartItem(cartId, courseId, unitPrice, titleSnapshot);

        // Assert
        Assert.Equal(cartId, cartItem.CartId);
        Assert.Equal(courseId, cartItem.CourseId);
        Assert.Equal(unitPrice, cartItem.UnitPrice);
        Assert.Equal(titleSnapshot, cartItem.TitleSnapshot);
        Assert.Null(cartItem.ThumbnailSnapshot);
        Assert.Null(cartItem.InstructorSnapshot);
    }

    [Fact]
    public void CartItem_UpdateSnapshot_UpdatesAllFields()
    {
        // Arrange
        var cartItem = CreateValidCartItem();
        var newTitle = "Updated Course Title";
        var newThumbnail = "https://example.com/new-thumbnail.jpg";
        var newInstructor = "Updated Instructor";

        // Act
        cartItem.UpdateSnapshot(newTitle, newThumbnail, newInstructor);

        // Assert
        Assert.Equal(newTitle, cartItem.TitleSnapshot);
        Assert.Equal(newThumbnail, cartItem.ThumbnailSnapshot);
        Assert.Equal(newInstructor, cartItem.InstructorSnapshot);
    }

    [Fact]
    public void CartItem_UpdateSnapshot_WithNullOptionalParameters_UpdatesCorrectly()
    {
        // Arrange
        var cartItem = CreateValidCartItem();
        var newTitle = "Updated Course Title";

        // Act
        cartItem.UpdateSnapshot(newTitle);

        // Assert
        Assert.Equal(newTitle, cartItem.TitleSnapshot);
        Assert.Null(cartItem.ThumbnailSnapshot);
        Assert.Null(cartItem.InstructorSnapshot);
    }

    private CartItem CreateValidCartItem()
    {
        return new CartItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100000L,
            "Original Title",
            "https://example.com/original-thumbnail.jpg",
            "Original Instructor"
        );
    }
}