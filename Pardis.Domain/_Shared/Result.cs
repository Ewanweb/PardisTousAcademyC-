namespace Pardis.Domain._Shared;

/// <summary>
/// Explicit result type with detailed failure states - NO BOOLEANS
/// </summary>
public class Result<T>
{
    public T? Value { get; private set; }
    public ResultState State { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string ErrorCode { get; private set; } = string.Empty;
    public Exception? Exception { get; private set; }

    private Result(T? value, ResultState state, string errorMessage = "", string errorCode = "", Exception? exception = null)
    {
        Value = value;
        State = state;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Exception = exception;
    }

    public static Result<T> Success(T value) => new(value, ResultState.Success);

    public static Result<T> NotFound(string message = "Resource not found", string code = "NOT_FOUND") => 
        new(default, ResultState.NotFound, message, code);

    public static Result<T> ValidationError(string message, string code = "VALIDATION_ERROR") => 
        new(default, ResultState.ValidationError, message, code);

    public static Result<T> AuthorizationError(string message = "Access denied", string code = "AUTHORIZATION_ERROR") => 
        new(default, ResultState.AuthorizationError, message, code);

    public static Result<T> ConcurrencyError(string message = "Concurrency conflict", string code = "CONCURRENCY_ERROR") => 
        new(default, ResultState.ConcurrencyError, message, code);

    public static Result<T> BusinessRuleViolation(string message, string code = "BUSINESS_RULE_VIOLATION") => 
        new(default, ResultState.BusinessRuleViolation, message, code);

    public static Result<T> SystemError(string message, Exception? exception = null, string code = "SYSTEM_ERROR") => 
        new(default, ResultState.SystemError, message, code, exception);

    public static Result<T> ExternalServiceError(string message, string code = "EXTERNAL_SERVICE_ERROR") => 
        new(default, ResultState.ExternalServiceError, message, code);

    public static Result<T> RateLimitExceeded(string message = "Rate limit exceeded", string code = "RATE_LIMIT_EXCEEDED") => 
        new(default, ResultState.RateLimitExceeded, message, code);

    public static Result<T> IdempotencyViolation(string message = "Idempotency key reused", string code = "IDEMPOTENCY_VIOLATION") => 
        new(default, ResultState.IdempotencyViolation, message, code);

    // Convenience methods
    public bool IsSuccess => State == ResultState.Success;
    public bool IsError => State != ResultState.Success;
    public bool IsNotFound => State == ResultState.NotFound;
    public bool IsValidationError => State == ResultState.ValidationError;
    public bool IsAuthorizationError => State == ResultState.AuthorizationError;
    public bool IsConcurrencyError => State == ResultState.ConcurrencyError;
    public bool IsBusinessRuleViolation => State == ResultState.BusinessRuleViolation;
    public bool IsSystemError => State == ResultState.SystemError;
    public bool IsExternalServiceError => State == ResultState.ExternalServiceError;
    public bool IsRateLimitExceeded => State == ResultState.RateLimitExceeded;
    public bool IsIdempotencyViolation => State == ResultState.IdempotencyViolation;

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, TResult> onError)
    {
        return IsSuccess ? onSuccess(Value!) : onError(ErrorMessage, ErrorCode);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<string, string, Task<TResult>> onError)
    {
        return IsSuccess ? await onSuccess(Value!) : await onError(ErrorMessage, ErrorCode);
    }
}

public enum ResultState
{
    Success = 1,
    NotFound = 2,
    ValidationError = 3,
    AuthorizationError = 4,
    ConcurrencyError = 5,
    BusinessRuleViolation = 6,
    SystemError = 7,
    ExternalServiceError = 8,
    RateLimitExceeded = 9,
    IdempotencyViolation = 10
}

/// <summary>
/// Result without value for operations that don't return data
/// </summary>
public class Result
{
    public ResultState State { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string ErrorCode { get; private set; } = string.Empty;
    public Exception? Exception { get; private set; }

    private Result(ResultState state, string errorMessage = "", string errorCode = "", Exception? exception = null)
    {
        State = state;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Exception = exception;
    }

    public static Result Success() => new(ResultState.Success);

    public static Result NotFound(string message = "Resource not found", string code = "NOT_FOUND") => 
        new(ResultState.NotFound, message, code);

    public static Result ValidationError(string message, string code = "VALIDATION_ERROR") => 
        new(ResultState.ValidationError, message, code);

    public static Result AuthorizationError(string message = "Access denied", string code = "AUTHORIZATION_ERROR") => 
        new(ResultState.AuthorizationError, message, code);

    public static Result ConcurrencyError(string message = "Concurrency conflict", string code = "CONCURRENCY_ERROR") => 
        new(ResultState.ConcurrencyError, message, code);

    public static Result BusinessRuleViolation(string message, string code = "BUSINESS_RULE_VIOLATION") => 
        new(ResultState.BusinessRuleViolation, message, code);

    public static Result SystemError(string message, Exception? exception = null, string code = "SYSTEM_ERROR") => 
        new(ResultState.SystemError, message, code, exception);

    public static Result ExternalServiceError(string message, string code = "EXTERNAL_SERVICE_ERROR") => 
        new(ResultState.ExternalServiceError, message, code);

    public static Result RateLimitExceeded(string message = "Rate limit exceeded", string code = "RATE_LIMIT_EXCEEDED") => 
        new(ResultState.RateLimitExceeded, message, code);

    public static Result IdempotencyViolation(string message = "Idempotency key reused", string code = "IDEMPOTENCY_VIOLATION") => 
        new(ResultState.IdempotencyViolation, message, code);

    // Convenience methods
    public bool IsSuccess => State == ResultState.Success;
    public bool IsError => State != ResultState.Success;
    public bool IsNotFound => State == ResultState.NotFound;
    public bool IsValidationError => State == ResultState.ValidationError;
    public bool IsAuthorizationError => State == ResultState.AuthorizationError;
    public bool IsConcurrencyError => State == ResultState.ConcurrencyError;
    public bool IsBusinessRuleViolation => State == ResultState.BusinessRuleViolation;
    public bool IsSystemError => State == ResultState.SystemError;
    public bool IsExternalServiceError => State == ResultState.ExternalServiceError;
    public bool IsRateLimitExceeded => State == ResultState.RateLimitExceeded;
    public bool IsIdempotencyViolation => State == ResultState.IdempotencyViolation;
}