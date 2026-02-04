IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}Message = error };
}

// Extension method for mocking DbSet
public static class MockExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<             return FileValidationResult.Invalid("محتوای فایل مشکوک است");
            }
        }

        return FileValidationResult.Valid("image/jpeg");
    }
}

// Helper class for testing
public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static FileValidationResult Valid(string mimeType) => new() { IsValid = true };
    public static FileValidationResult Invalid(string error) => new() { IsValid = false, Errorssert.True(result.IsValid);
    }

    private FileValidationResult ValidateFileContent(byte[] bytes)
    {
        // Simplified validation logic for testing
        var suspiciousPatterns = new[]
        {
            new byte[] { 0x4D, 0x5A }, // PE header
            System.Text.Encoding.ASCII.GetBytes("<script"),
            System.Text.Encoding.ASCII.GetBytes("<?php")
        };

        foreach (var pattern in suspiciousPatterns)
        {
            if (bytes.AsSpan().IndexOf(pattern) >= 0)
            {
   ss')</script>");

        // Act & Assert - MUST reject script content
        var result = ValidateFileContent(scriptBytes);
        Assert.False(result.IsValid);
        Assert.Contains("مشکوک", result.ErrorMessage);
    }

    [Fact]
    public void FileValidation_WithValidImage_ShouldAccept()
    {
        // Arrange
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header

        // Act & Assert - MUST accept valid images
        var result = ValidateFileContent(jpegBytes);
        AxecutableFile_ShouldReject()
    {
        // Arrange
        var executableBytes = new byte[] { 0x4D, 0x5A }; // PE header

        // Act & Assert - MUST reject executable files
        var result = ValidateFileContent(executableBytes);
        Assert.False(result.IsValid);
        Assert.Contains("مشکوک", result.ErrorMessage);
    }

    [Fact]
    public void FileValidation_WithScriptContent_ShouldReject()
    {
        // Arrange
        var scriptBytes = System.Text.Encoding.UTF8.GetBytes("<script>alert('xrify(x => x.AddAsync(
            It.Is<PaymentAuditLog>(log => 
                log.PaymentAttemptId == paymentAttemptId &&
                log.AdminUserId == "admin123" &&
                log.IdempotencyKey == "audit-test-123" &&
                log.IpAddress == "192.168.1.1"),
            It.IsAny<CancellationToken>()), 
            Times.AtLeastOnce);
    }
}

/// <summary>
/// File upload security tests
/// </summary>
public class FileUploadSecurityTests
{
    [Fact]
    public void FileValidation_WithE  async (operation, ct) => await operation(ct));

        var command = new AdminReviewPaymentCommand
        {
            PaymentAttemptId = paymentAttemptId,
            AdminUserId = "admin123",
            IsApproved = true,
            IdempotencyKey = "audit-test-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - MUST create audit log
        _auditRepository.VeeateAuditLog()
    {
        // Arrange
        var paymentAttemptId = Guid.NewGuid();
        var paymentAttempt = new PaymentAttempt(Guid.NewGuid(), "user123", 100000);

        _paymentAttemptRepository.Setup(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<OperationResult<AdminReviewPaymentResult>>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<OperationResult<AdminReviewPaymentResult>>>, CancellationToken>(
               = "192.168.1.1",
            UserAgent = "TestAgent"
        };

        // Act & Assert - MUST rollback on enrollment failure
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Verify rollback occurred
        _enrollmentRepository.Verify(x => x.DeleteAsync(It.IsAny<CourseEnrollment>(), It.IsAny<CancellationToken>()), 
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task AdminReviewPayment_ShouldCr123", new Cart("user123"));

        // Simulate enrollment failure
        _enrollmentRepository.Setup(x => x.AddAsync(It.IsAny<CourseEnrollment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Enrollment failed"));

        var command = new AdminReviewPaymentCommand
        {
            PaymentAttemptId = paymentAttemptId,
            AdminUserId = "admin123",
            IsApproved = true,
            IdempotencyKey = "rollback-test-123",
            IpAddressr result = await _handler.Handle(command, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("عملیات همزمان دیگری در حال انجام است", result.Message);
    }

    [Fact]
    public async Task AdminReviewPayment_EnrollmentFailure_ShouldRollbackTransaction()
    {
        // Arrange
        var paymentAttemptId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var paymentAttempt = new PaymentAttempt(orderId, "user123", 100000);
        var order = new Order("userk<OperationResult<AdminReviewPaymentResult>>>, CancellationToken>(
                async (operation, ct) => await operation(ct));

        var command = new AdminReviewPaymentCommand
        {
            PaymentAttemptId = paymentAttemptId,
            AdminUserId = "admin123",
            IsApproved = true,
            IdempotencyKey = "unique-key-123",
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent"
        };

        // Act & Assert - MUST handle concurrency gracefully
        va     // Simulate optimistic locking conflict
        _paymentAttemptRepository.Setup(x => x.UpdateAsync(It.IsAny<PaymentAttempt>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("Concurrency conflict"));

        _paymentAttemptRepository.Setup(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<OperationResult<AdminReviewPaymentResult>>>>(),
            It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Tas    // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - MUST return cached result
        Assert.True(result.IsSuccess);
        Assert.Contains("قبلاً انجام شده است", result.Message);
    }

    [Fact]
    public async Task AdminReviewPayment_ConcurrentRequests_ShouldHandleOptimisticLocking()
    {
        // Arrange
        var paymentAttemptId = Guid.NewGuid();
        var paymentAttempt = new PaymentAttempt(Guid.NewGuid(), "user123", 100000);
        
   ockDbSet().Object);

        _paymentAttemptRepository.Setup(x => x.GetByIdAsync(paymentAttemptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentAttempt);

        var command = new AdminReviewPaymentCommand
        {
            PaymentAttemptId = paymentAttemptId,
            AdminUserId = adminUserId,
            IsApproved = true,
            IdempotencyKey = idempotencyKey, // CRITICAL: Duplicate key
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent"
        };

    w PaymentAuditLog(
            paymentAttemptId,
            "user123",
            PaymentAuditAction.AdminApproved,
            "AwaitingAdminApproval",
            "Paid",
            100000,
            "192.168.1.1",
            "TestAgent",
            adminUserId,
            null,
            idempotencyKey);

        var paymentAttempt = new PaymentAttempt(Guid.NewGuid(), "user123", 100000);

        _auditRepository.Setup(x => x.Table)
            .Returns(new[] { existingAudit }.AsQueryable().BuildM var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - MUST FAIL
        Assert.False(result.IsSuccess);
        Assert.Contains("Idempotency key is required", result.Message);
    }

    [Fact]
    public async Task AdminReviewPayment_WithDuplicateIdempotencyKey_ShouldReturnCachedResult()
    {
        // Arrange
        var paymentAttemptId = Guid.NewGuid();
        var idempotencyKey = "test-key-123";
        var adminUserId = "admin123";

        var existingAudit = neory.Object,
            _logger.Object);
    }

    [Fact]
    public async Task AdminReviewPayment_WithoutIdempotencyKey_ShouldFail()
    {
        // Arrange
        var command = new AdminReviewPaymentCommand
        {
            PaymentAttemptId = Guid.NewGuid(),
            AdminUserId = "admin123",
            IsApproved = true,
            IdempotencyKey = "", // CRITICAL: Empty idempotency key
            IpAddress = "192.168.1.1",
            UserAgent = "TestAgent"
        };

        // Act
       ck<IOrderRepository>();
        _cartRepository = new Mock<ICartRepository>();
        _enrollmentRepository = new Mock<ICourseEnrollmentRepository>();
        _auditRepository = new Mock<IPaymentAuditLogRepository>();
        _logger = new Mock<ILogger<AdminReviewPaymentHandler>>();

        _handler = new AdminReviewPaymentHandler(
            _paymentAttemptRepository.Object,
            _orderRepository.Object,
            _cartRepository.Object,
            _enrollmentRepository.Object,
            _auditRepositRepository> _orderRepository;
    private readonly Mock<ICartRepository> _cartRepository;
    private readonly Mock<ICourseEnrollmentRepository> _enrollmentRepository;
    private readonly Mock<IPaymentAuditLogRepository> _auditRepository;
    private readonly Mock<ILogger<AdminReviewPaymentHandler>> _logger;
    private readonly AdminReviewPaymentHandler _handler;

    public PaymentSecurityTests()
    {
        _paymentAttemptRepository = new Mock<IPaymentAttemptRepository>();
        _orderRepository = new Moing Pardis.Domain.Shopping;
using Xunit;

namespace Pardis.Application.Tests.Security;

/// <summary>
/// CRITICAL: Security tests for payment operations
/// These tests MUST pass before production deployment
/// </summary>
public class PaymentSecurityTests
{
    private readonly Mock<IPaymentAttemptRepository> _paymentAttemptRepository;
    private readonly Mock<IOrderg Microsoft.Extensions.Logging;
using Moq;
using Pardis.Application.Shopping.PaymentAttempts.AdminReviewPayment;
using Pardis.Application._Shared;
using Pardis.Domain.Audit;
using Pardis.Domain.Payments;
ususing Microsoft.EntityFrameworkCore;
usin