using Microsoft.Extensions.Options;

namespace Pardis.Application._Shared;

/// <summary>
/// Centralized date/time service with timezone handling
/// </summary>
public class DateTimeService : IDateTimeService
{
    private readonly DateTimeOptions _options;
    private readonly TimeZoneInfo _applicationTimeZone;

    public DateTimeService(IOptions<DateTimeOptions> options)
    {
        _options = options.Value;
        _applicationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.ApplicationTimeZone);
    }

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime ApplicationNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _applicationTimeZone);

    public DateTime ToApplicationTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("DateTime must be UTC", nameof(utcDateTime));

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _applicationTimeZone);
    }

    public DateTime ToUtc(DateTime applicationDateTime)
    {
        if (applicationDateTime.Kind == DateTimeKind.Utc)
            return applicationDateTime;

        return TimeZoneInfo.ConvertTimeToUtc(applicationDateTime, _applicationTimeZone);
    }

    public string FormatForDisplay(DateTime utcDateTime, string format = "yyyy/MM/dd HH:mm")
    {
        var localTime = ToApplicationTime(utcDateTime);
        return localTime.ToString(format);
    }

    public DateTime StartOfDay(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public DateTime EndOfDay(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Utc);
    }
}

public class DateTimeOptions
{
    public string ApplicationTimeZone { get; set; } = "Iran Standard Time";
}

public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateTime ApplicationNow { get; }
    DateTime ToApplicationTime(DateTime utcDateTime);
    DateTime ToUtc(DateTime applicationDateTime);
    string FormatForDisplay(DateTime utcDateTime, string format = "yyyy/MM/dd HH:mm");
    DateTime StartOfDay(DateTime dateTime);
    DateTime EndOfDay(DateTime dateTime);
}