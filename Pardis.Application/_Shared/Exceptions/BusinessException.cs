namespace Pardis.Application._Shared.Exceptions;

/// <summary>
/// استثناء منطق کسب‌وکار
/// </summary>
public class BusinessException : Exception
{
    public string UserFriendlyMessage { get; }

    public BusinessException(string userFriendlyMessage) : base(userFriendlyMessage)
    {
        UserFriendlyMessage = userFriendlyMessage;
    }

    public BusinessException(string userFriendlyMessage, Exception innerException) 
        : base(userFriendlyMessage, innerException)
    {
        UserFriendlyMessage = userFriendlyMessage;
    }
}