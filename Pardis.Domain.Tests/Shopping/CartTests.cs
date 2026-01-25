using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Xunit;

namespace Pardis.Domain.Tests.Shopping;

public class CartTests
{
    [Fact]
    public void Cart_AddCourse_WithValidCourse_AddsItemWithCorrectSnapshots()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var instructor = new User { Id = "instructor-id", FirstName = "John", LastName = "Doe" };
        var course = new Course(
            title: "Test Course",
            slug: "test-course",
            description: "Test Description",
            price: 100000,
            thumbnail: "test-thumbnail.jpg",
            status: CourseStatus.Published,
            type: CourseType.Online,
            location: "Online",
            instructorId: instructor.Id,
            schedule: "Weekends",
            isStarted: false,
            isCompleted: false,
            startFrom: "2024-01-01",
            categoryId: Guid.NewGuid(),
            seo: new Pardis.Domain.Seo.SeoMetadata()
        );
        
        // Set instructor navigation property
        typeof(Course).GetProperty("Instructor")!.SetValue(course, instructor);

        // Act
        cart.AddCourse(course);

        // Assert
        Assert.Single(cart.Items);
        var addedItem = cart.Items.First();
        
        Assert.Equal(course.Id, addedItem.CourseId);
        Assert.Equal(course.Title, addedItem.TitleSnapshot);
        Assert.Equal(course.Thumbnail, addedItem.ThumbnailSnapshot);
        Assert.Equal(instructor.FullName, addedItem.InstructorSnapshot);
        Assert.Equal(course.Price, addedItem.UnitPrice);
        Assert.Equal(course.Price, cart.TotalAmount);
    }

    [Fact]
    public void Cart_AddCourse_WithNullThumbnail_UsesDefaultThumbnail()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var instructor = new User { Id = "instructor-id", FirstName = "Jane", LastName = "Smith" };
        var course = new Course(
            title: "Test Course",
            slug: "test-course",
            description: "Test Description",
            price: 50000,
            thumbnail: null, // No thumbnail
            status: CourseStatus.Published,
            type: CourseType.Online,
            location: "Online",
            instructorId: instructor.Id,
            schedule: "Weekdays",
            isStarted: false,
            isCompleted: false,
            startFrom: "2024-02-01",
            categoryId: Guid.NewGuid(),
            seo: new Pardis.Domain.Seo.SeoMetadata()
        );
        
        // Set instructor navigation property
        typeof(Course).GetProperty("Instructor")!.SetValue(course, instructor);

        // Act
        cart.AddCourse(course);

        // Assert
        var addedItem = cart.Items.First();
        Assert.Equal("/images/default-course-thumbnail.jpg", addedItem.ThumbnailSnapshot);
        Assert.Equal(instructor.FullName, addedItem.InstructorSnapshot);
    }

    [Fact]
    public void Cart_AddCourse_WithNullInstructor_UsesDefaultInstructor()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var course = new Course(
            title: "Test Course",
            slug: "test-course",
            description: "Test Description",
            price: 75000,
            thumbnail: "test-thumbnail.jpg",
            status: CourseStatus.Published,
            type: CourseType.Online,
            location: "Online",
            instructorId: "instructor-id",
            schedule: "Flexible",
            isStarted: false,
            isCompleted: false,
            startFrom: "2024-03-01",
            categoryId: Guid.NewGuid(),
            seo: new Pardis.Domain.Seo.SeoMetadata()
        );
        
        // Don't set instructor navigation property (null)

        // Act
        cart.AddCourse(course);

        // Assert
        var addedItem = cart.Items.First();
        Assert.Equal("نامشخص", addedItem.InstructorSnapshot);
    }

    [Fact]
    public void Cart_AddCourse_DuplicateCourse_ThrowsException()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var course = CreateValidCourse();

        // Act & Assert
        cart.AddCourse(course);
        
        var exception = Assert.Throws<InvalidOperationException>(() => cart.AddCourse(course));
        Assert.Contains("این دوره قبلاً به سبد خرید اضافه شده است", exception.Message);
    }

    [Fact]
    public void Cart_AddCourse_WithEmptyCartId_ThrowsException()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        // Don't set cart ID (remains Guid.Empty)
        
        var course = CreateValidCourse();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => cart.AddCourse(course));
        Assert.Contains("سبد خرید باید قبل از اضافه کردن آیتم ذخیره شود", exception.Message);
    }

    [Fact]
    public void Cart_RemoveCourse_ExistingCourse_RemovesSuccessfully()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var course = CreateValidCourse();
        cart.AddCourse(course);

        // Act
        cart.RemoveCourse(course.Id);

        // Assert
        Assert.Empty(cart.Items);
        Assert.Equal(0, cart.TotalAmount);
    }

    [Fact]
    public void Cart_RemoveCourse_NonExistentCourse_ThrowsException()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        var nonExistentCourseId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => cart.RemoveCourse(nonExistentCourseId));
        Assert.Contains("دوره در سبد خرید یافت نشد", exception.Message);
    }

    [Fact]
    public void Cart_Clear_RemovesAllItems()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Simulate saving cart to get ID
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        
        var course1 = CreateValidCourse();
        var course2 = CreateValidCourse();
        typeof(Course).GetProperty("Id")!.SetValue(course2, Guid.NewGuid());
        
        cart.AddCourse(course1);
        cart.AddCourse(course2);

        // Act
        cart.Clear();

        // Assert
        Assert.Empty(cart.Items);
        Assert.Equal(0, cart.TotalAmount);
    }

    [Fact]
    public void Cart_IsExpired_WithExpiredDate_ReturnsTrue()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Set expiry to past date
        typeof(Cart).GetProperty("ExpiresAt")!.SetValue(cart, DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        Assert.True(cart.IsExpired());
    }

    [Fact]
    public void Cart_IsExpired_WithFutureDate_ReturnsFalse()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        
        // Default expiry is 7 days from now
        // Act & Assert
        Assert.False(cart.IsExpired());
    }

    [Fact]
    public void Cart_ExtendExpiry_UpdatesExpiryDate()
    {
        // Arrange
        var userId = "test-user-id";
        var cart = new Cart(userId);
        var originalExpiry = cart.ExpiresAt;

        // Act
        cart.ExtendExpiry(14); // Extend by 14 days

        // Assert
        Assert.True(cart.ExpiresAt > originalExpiry);
        Assert.True(cart.ExpiresAt >= DateTime.UtcNow.AddDays(13)); // Allow some margin for execution time
    }

    private Course CreateValidCourse()
    {
        var instructor = new User { Id = "instructor-id", FirstName = "Test", LastName = "Instructor" };
        var course = new Course(
            title: "Valid Test Course",
            slug: "valid-test-course",
            description: "Valid Test Description",
            price: 100000,
            thumbnail: "valid-thumbnail.jpg",
            status: CourseStatus.Published,
            type: CourseType.Online,
            location: "Online",
            instructorId: instructor.Id,
            schedule: "Flexible",
            isStarted: false,
            isCompleted: false,
            startFrom: "2024-01-01",
            categoryId: Guid.NewGuid(),
            seo: new Pardis.Domain.Seo.SeoMetadata()
        );
        
        // Set instructor navigation property
        typeof(Course).GetProperty("Instructor")!.SetValue(course, instructor);
        
        return course;
    }
}