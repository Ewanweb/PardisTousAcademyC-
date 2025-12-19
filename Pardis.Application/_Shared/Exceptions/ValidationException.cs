namespace Pardis.Application._Shared.Exceptions;

/// <summary>
/// استثناء اعتبارسنجی
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors) : base("خطا در اعتبارسنجی داده‌ها")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error) : base("خطا در اعتبارسنجی داده‌ها")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }
}