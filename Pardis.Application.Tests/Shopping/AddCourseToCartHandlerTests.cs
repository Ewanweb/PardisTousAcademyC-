using Xunit;
using Moq;
using AutoMapper;
using Pardis.Application.Shopping.Cart.AddCourseToCart;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Courses.Contracts;
using Pardis.Application.Payments.Contracts;
using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;
using Pardis.Domain.Payments;

namespace Pardis.Application.Tests.Shopping;

public class AddCourseToCartHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AddCourseToCartHandler _handler;

    public AddCourseToCartHandlerTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _handler = new AddCourseToCartHandler(
            _cartRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _enrollmentRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_AddsToExistingCart_ReturnsSuccess()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        var course = CreateMockCourse(courseId, "Test Course", 100000L);
        var existingCart = CreateMockCart(userId);
        
        var command = new AddCourseToCartCommand { UserId = userId, CourseId = courseId };

        _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _enrollmentRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseEnrollment?)null);
        
        _cartRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(existingCart.Id, result.Data.CartId);
        Assert.Equal(1, result.Data.TotalItems);
        Assert.Equal(100000L, result.Data.TotalAmount);
        
        _cartRepositoryMock.Verify(x => x.UpdateAsync(existingCart, It.IsAny<CancellationToken>()), Times.Once);
        _cartRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesNewCart_ReturnsSuccess()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        var course = CreateMockCourse(courseId, "Test Course", 100000L);
        var newCart = CreateMockCart(userId);
        
        var command = new AddCourseToCartCommand { UserId = userId, CourseId = courseId };

        _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _enrollmentRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseEnrollment?)null);
        
        _cartRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);
        
        _cartRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        
        _cartRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
        _cartRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2)); // Once for create, once for update
    }

    [Fact]
    public async Task Handle_WithNonExistentCourse_ReturnsNotFound()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        var command = new AddCourseToCartCommand { UserId = userId, CourseId = courseId };

        _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("دوره یافت نشد", result.Message);
        
        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingEnrollment_ReturnsError()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        var course = CreateMockCourse(courseId, "Test Course", 100000L);
        var existingEnrollment = CreateMockEnrollment(userId, courseId);
        
        var command = new AddCourseToCartCommand { UserId = userId, CourseId = courseId };

        _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _enrollmentRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEnrollment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("شما قبلاً در این دوره ثبت‌نام کرده‌اید", result.Message);
        
        _cartRepositoryMock.Verify(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCourseAlreadyInCart_ReturnsError()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        var course = CreateMockCourse(courseId, "Test Course", 100000L);
        var existingCart = CreateMockCart(userId);
        
        // Mock cart to contain the course
        existingCart.Setup(x => x.ContainsCourse(courseId)).Returns(true);
        
        var command = new AddCourseToCartCommand { UserId = userId, CourseId = courseId };

        _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _enrollmentRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseEnrollment?)null);
        
        _cartRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCart.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("این دوره قبلاً به سبد خرید اضافه شده است", result.Message);
        
        _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private Course CreateMockCourse(Guid courseId, string title, long price)
    {
        var courseMock = new Mock<Course>();
        courseMock.Setup(x => x.Id).Returns(courseId);
        courseMock.Setup(x => x.Title).Returns(title);
        courseMock.Setup(x => x.Price).Returns(price);
        return courseMock.Object;
    }

    private Mock<Cart> CreateMockCart(string userId)
    {
        var cartMock = new Mock<Cart>();
        cartMock.Setup(x => x.Id).Returns(Guid.NewGuid());
        cartMock.Setup(x => x.UserId).Returns(userId);
        cartMock.Setup(x => x.GetItemCount()).Returns(1);
        cartMock.Setup(x => x.TotalAmount).Returns(100000L);
        cartMock.Setup(x => x.ContainsCourse(It.IsAny<Guid>())).Returns(false);
        
        // Mock Items collection
        var cartItemMock = new Mock<CartItem>();
        cartItemMock.Setup(x => x.Id).Returns(Guid.NewGuid());
        cartItemMock.Setup(x => x.CourseId).Returns(Guid.NewGuid());
        
        var items = new List<CartItem> { cartItemMock.Object };
        cartMock.Setup(x => x.Items).Returns(items);
        
        return cartMock;
    }

    private CourseEnrollment CreateMockEnrollment(string userId, Guid courseId)
    {
        var enrollmentMock = new Mock<CourseEnrollment>();
        enrollmentMock.Setup(x => x.UserId).Returns(userId);
        enrollmentMock.Setup(x => x.CourseId).Returns(courseId);
        return enrollmentMock.Object;
    }
}