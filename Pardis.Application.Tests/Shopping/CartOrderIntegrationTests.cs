using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Pardis.Application.Shopping.Cart.AddCourseToCart;
using Pardis.Application.Shopping.Checkout.CreateCheckout;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Shopping.Validation;
using Pardis.Application.Payments.Contracts;
using Pardis.Application.Courses.Contracts;
using Pardis.Domain.Shopping;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Pardis.Domain;

namespace Pardis.Application.Tests.Shopping;

public class CartOrderIntegrationTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;
    private readonly Mock<IRepository<UserCourse>> _mockUserCourseRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<ICartValidationService> _mockValidationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<AddCourseToCartHandler>> _mockLogger;

    public CartOrderIntegrationTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
        _mockUserCourseRepository = new Mock<IRepository<UserCourse>>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockValidationService = new Mock<ICartValidationService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<AddCourseToCartHandler>>();
    }

    [Fact]
    public async Task AddCourseToCart_WithValidCourse_CreatesCartItemWithCorrectSnapshots()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        
        var instructor = new User { Id = "instructor-id", FirstName = "John", LastName = "Doe" };
        var course = CreateValidCourse(courseId, instructor);
        
        var validationResult = new CartValidationResult
        {
            IsValid = true,
            Course = course
        };

        _mockValidationService
            .Setup(x => x.ValidateAddCourseToCartAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _mockCartRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var newCart = new Cart(userId);
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(newCart, cartId);

        _mockCartRepository
            .Setup(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCart);

        _mockCartRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockCartRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddCourseToCartHandler(
            _mockCartRepository.Object,
            _mockValidationService.Object,
            _mockLogger.Object,
            _mockMapper.Object
        );

        var command = new AddCourseToCartCommand
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(cartId, result.Data.CartId);
        Assert.Equal(1, result.Data.TotalItems);
        Assert.Equal(course.Price, result.Data.TotalAmount);
        Assert.Equal(course.Title, result.Data.CourseTitle);
        Assert.Equal(course.Price, result.Data.CoursePrice);

        // Verify cart operations
        _mockCartRepository.Verify(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddCourseToCart_WithExistingCart_AddsToExistingCart()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();
        
        var instructor = new User { Id = "instructor-id", FirstName = "Jane", LastName = "Smith" };
        var course = CreateValidCourse(courseId, instructor);
        
        var existingCart = new Cart(userId);
        var cartId = Guid.NewGuid();
        typeof(Cart).GetProperty("Id")!.SetValue(existingCart, cartId);

        var validationResult = new CartValidationResult
        {
            IsValid = true,
            Course = course
        };

        _mockValidationService
            .Setup(x => x.ValidateAddCourseToCartAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _mockCartRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCart);

        _mockCartRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockCartRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddCourseToCartHandler(
            _mockCartRepository.Object,
            _mockValidationService.Object,
            _mockLogger.Object,
            _mockMapper.Object
        );

        var command = new AddCourseToCartCommand
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cartId, result.Data.CartId);

        // Verify no new cart was created
        _mockCartRepository.Verify(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockCartRepository.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCourseToCart_WithValidationError_ReturnsError()
    {
        // Arrange
        var userId = "test-user-id";
        var courseId = Guid.NewGuid();

        var validationResult = new CartValidationResult
        {
            IsValid = false
        };
        validationResult.AddError("COURSE_NOT_FOUND", "دوره مورد نظر یافت نشد");

        _mockValidationService
            .Setup(x => x.ValidateAddCourseToCartAsync(userId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var handler = new AddCourseToCartHandler(
            _mockCartRepository.Object,
            _mockValidationService.Object,
            _mockLogger.Object,
            _mockMapper.Object
        );

        var command = new AddCourseToCartCommand
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("دوره مورد نظر یافت نشد", result.Message);

        // Verify no cart operations were performed
        _mockCartRepository.Verify(x => x.CreateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockCartRepository.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCheckout_WithValidCart_CreatesOrderWithCorrectCartSnapshot()
    {
        // Arrange
        var userId = "test-user-id";
        var cartId = Guid.NewGuid();
        
        var instructor = new User { Id = "instructor-id", FirstName = "Test", LastName = "Instructor" };
        var course = CreateValidCourse(Guid.NewGuid(), instructor);
        
        var cart = new Cart(userId);
        typeof(Cart).GetProperty("Id")!.SetValue(cart, cartId);
        cart.AddCourse(course);

        _mockCartRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _mockEnrollmentRepository
            .Setup(x => x.GetByUserAndCourseAsync(userId, course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseEnrollment?)null);

        var createdOrder = new Order(userId, cart);
        _mockOrderRepository
            .Setup(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdOrder);

        _mockOrderRepository
            .Setup(x => x.ExecuteInTransactionAsync<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>(
                It.IsAny<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>, CancellationToken>(
                (func, ct) => func(ct));

        var handler = new CreateCheckoutHandler(
            _mockCartRepository.Object,
            _mockOrderRepository.Object,
            _mockEnrollmentRepository.Object,
            _mockUserCourseRepository.Object,
            _mockMapper.Object
        );

        var command = new CreateCheckoutCommand
        {
            UserId = userId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(createdOrder.Id, result.Data.OrderId);
        Assert.Equal(createdOrder.OrderNumber, result.Data.OrderNumber);
        Assert.Equal(cart.TotalAmount, result.Data.TotalAmount);

        // Verify order creation
        _mockOrderRepository.Verify(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCheckout_WithEmptyCart_ReturnsError()
    {
        // Arrange
        var userId = "test-user-id";

        _mockCartRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        _mockOrderRepository
            .Setup(x => x.ExecuteInTransactionAsync<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>(
                It.IsAny<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>, CancellationToken>(
                (func, ct) => func(ct));

        var handler = new CreateCheckoutHandler(
            _mockCartRepository.Object,
            _mockOrderRepository.Object,
            _mockEnrollmentRepository.Object,
            _mockUserCourseRepository.Object,
            _mockMapper.Object
        );

        var command = new CreateCheckoutCommand
        {
            UserId = userId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("سبد خرید خالی است", result.Message);

        // Verify no order was created
        _mockOrderRepository.Verify(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCheckout_WithExpiredCart_ReturnsError()
    {
        // Arrange
        var userId = "test-user-id";
        
        var cart = new Cart(userId);
        // Set cart as expired
        typeof(Cart).GetProperty("ExpiresAt")!.SetValue(cart, DateTime.UtcNow.AddDays(-1));

        _mockCartRepository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _mockOrderRepository
            .Setup(x => x.ExecuteInTransactionAsync<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>(
                It.IsAny<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Pardis.Application._Shared.OperationResult<CreateCheckoutResult>>>, CancellationToken>(
                (func, ct) => func(ct));

        var handler = new CreateCheckoutHandler(
            _mockCartRepository.Object,
            _mockOrderRepository.Object,
            _mockEnrollmentRepository.Object,
            _mockUserCourseRepository.Object,
            _mockMapper.Object
        );

        var command = new CreateCheckoutCommand
        {
            UserId = userId
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("سبد خرید منقضی شده است", result.Message);
    }

    private Course CreateValidCourse(Guid courseId, User instructor)
    {
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
            schedule: "Flexible",
            isStarted: false,
            isCompleted: false,
            startFrom: "2024-01-01",
            categoryId: Guid.NewGuid(),
            seo: new Pardis.Domain.Seo.SeoMetadata()
        );
        
        // Set course ID and instructor
        typeof(Course).GetProperty("Id")!.SetValue(course, courseId);
        typeof(Course).GetProperty("Instructor")!.SetValue(course, instructor);
        
        return course;
    }
}