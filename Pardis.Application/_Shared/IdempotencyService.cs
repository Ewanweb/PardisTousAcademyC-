using Microsoft.Extensions.Logging;
using Pardis.Domain.Idempotency;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Pardis.Application._Shared;

/// <summary>
/// Service for handling idempotency across all operations
/// </summary>
public class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyRepository _repository;
    private readonly ILogger<IdempotencyService> _logger;

    public IdempotencyService(IIdempotencyRepository repository, ILogger<IdempotencyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IdempotencyResult<T>> ExecuteWithIdempotencyAsync<T>(
        string idempotencyKey,
        string userId,
        string operationType,
        object request,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
            return IdempotencyResult<T>.Error("Idempotency key is required");

        try
        {
            // Generate request hash for validation
            var requestHash = ComputeRequestHash(request);

            // Check for existing record
            var existingRecord = await _repository.GetByKeyAsync(idempotencyKey, userId, operationType, cancellationToken);

            if (existingRecord != null)
            {
                // Validate request hasn't changed
                if (existingRecord.RequestHash != requestHash)
                {
                    _logger.LogWarning("Idempotency key reused with different request. Key: {Key}, UserId: {UserId}, Operation: {Operation}",
                        idempotencyKey, userId, operationType);
                    return IdempotencyResult<T>.Error("Idempotency key reused with different request parameters");
                }

                // Check if expired
                if (existingRecord.IsExpired())
                {
                    _logger.LogInformation("Expired idempotency record found. Key: {Key}, UserId: {UserId}",
                        idempotencyKey, userId);
                    
                    // Delete expired record and continue with new operation
                    _repository.Remove(existingRecord);
                    await _repository.SaveChangesAsync(cancellationToken);
                }
                else if (existingRecord.CanReplay())
                {
                    // Return cached result
                    _logger.LogInformation("Replaying idempotent operation. Key: {Key}, UserId: {UserId}, Operation: {Operation}",
                        idempotencyKey, userId, operationType);

                    if (!string.IsNullOrEmpty(existingRecord.ErrorMessage))
                    {
                        return IdempotencyResult<T>.Error(existingRecord.ErrorMessage);
                    }

                    var cachedResult = JsonSerializer.Deserialize<T>(existingRecord.ResponseData);
                    return IdempotencyResult<T>.Success(cachedResult!, true);
                }
                else
                {
                    // Operation in progress or failed
                    return IdempotencyResult<T>.Error("Operation already in progress or failed");
                }
            }

            // Create new idempotency record
            var record = new IdempotencyRecord(idempotencyKey, userId, operationType, requestHash);
            await _repository.AddAsync(record);
            await _repository.SaveChangesAsync(cancellationToken);

            try
            {
                // Execute operation
                var result = await operation(cancellationToken);

                // Mark as completed
                var responseData = JsonSerializer.Serialize(result);
                record.MarkCompleted(responseData, 200);
                _repository.Update(record);
                await _repository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Idempotent operation completed successfully. Key: {Key}, UserId: {UserId}, Operation: {Operation}",
                    idempotencyKey, userId, operationType);

                return IdempotencyResult<T>.Success(result, false);
            }
            catch (Exception ex)
            {
                // Mark as failed
                record.MarkFailed(ex.Message, 500);
                _repository.Update(record);
                await _repository.SaveChangesAsync(cancellationToken);

                _logger.LogError(ex, "Idempotent operation failed. Key: {Key}, UserId: {UserId}, Operation: {Operation}",
                    idempotencyKey, userId, operationType);

                throw;
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Error in idempotency service. Key: {Key}, UserId: {UserId}, Operation: {Operation}",
                idempotencyKey, userId, operationType);
            return IdempotencyResult<T>.Error("Internal error in idempotency handling");
        }
    }

    public async Task<bool> IsOperationCompletedAsync(
        string idempotencyKey,
        string userId,
        string operationType,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetByKeyAsync(idempotencyKey, userId, operationType, cancellationToken);
        return record?.IsCompleted == true && !record.IsExpired();
    }

    public async Task CleanupExpiredRecordsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _repository.CleanupExpiredAsync(cancellationToken);
            _logger.LogInformation("Expired idempotency records cleaned up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired idempotency records");
        }
    }

    private string ComputeRequestHash(object request)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hash);
    }
}

public class IdempotencyResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public bool IsReplayed { get; set; }
    public string? ErrorMessage { get; set; }

    public static IdempotencyResult<T> Success(T data, bool isReplayed) => 
        new() { IsSuccess = true, Data = data, IsReplayed = isReplayed };

    public static IdempotencyResult<T> Error(string message) => 
        new() { IsSuccess = false, ErrorMessage = message };
}

public interface IIdempotencyService
{
    Task<IdempotencyResult<T>> ExecuteWithIdempotencyAsync<T>(
        string idempotencyKey,
        string userId,
        string operationType,
        object request,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);

    Task<bool> IsOperationCompletedAsync(
        string idempotencyKey,
        string userId,
        string operationType,
        CancellationToken cancellationToken = default);

    Task CleanupExpiredRecordsAsync(CancellationToken cancellationToken = default);
}