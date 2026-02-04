namespace Pardis.Domain.Idempotency;

/// <summary>
/// Idempotency record for preventing duplicate operations
/// </summary>
public class IdempotencyRecord : BaseEntity
{
    public string IdempotencyKey { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string OperationType { get; private set; } = string.Empty; // PaymentApproval, ReceiptUpload, etc.
    public string RequestHash { get; private set; } = string.Empty; // Hash of request parameters
    public string ResponseData { get; private set; } = string.Empty; // JSON response for replay
    public int StatusCode { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsCompleted { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Private constructor for EF Core
    private IdempotencyRecord() { }

    public IdempotencyRecord(
        string idempotencyKey,
        string userId,
        string operationType,
        string requestHash,
        TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
            throw new ArgumentException("Idempotency key cannot be empty", nameof(idempotencyKey));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        Id = Guid.NewGuid();
        IdempotencyKey = idempotencyKey;
        UserId = userId;
        OperationType = operationType;
        RequestHash = requestHash;
        ExpiresAt = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(24));
        IsCompleted = false;
        ResponseData = string.Empty;
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(string responseData, int statusCode)
    {
        ResponseData = responseData;
        StatusCode = statusCode;
        IsCompleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string errorMessage, int statusCode)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        IsCompleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool CanReplay() => IsCompleted && !IsExpired();
}

public interface IIdempotencyRepository : IRepository<IdempotencyRecord>
{
    Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey, string userId, string operationType, CancellationToken cancellationToken = default);
    Task CleanupExpiredAsync(CancellationToken cancellationToken = default);
}