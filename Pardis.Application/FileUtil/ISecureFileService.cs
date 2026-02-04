using Microsoft.AspNetCore.Http;

namespace Pardis.Application.FileUtil;

public interface ISecureFileService
{
    Task<SecureFileUploadResult> SaveFileSecurely(IFormFile file, string category, string? userId = null);
    Task<SecureFileDownloadResult> GetFileSecurely(string accessToken, string? userId = null);
}

public class SecureFileUploadResult
{
    public bool IsSuccess { get; set; }
    public string? SecureFileName { get; set; }
    public string? AccessToken { get; set; }
    public string? Category { get; set; }
    public string? OriginalFileName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public static SecureFileUploadResult Error(string message) => new() { IsSuccess = false, ErrorMessage = message };
}

public class SecureFileDownloadResult
{
    public bool IsSuccess { get; set; }
    public byte[]? FileBytes { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
    public string? ErrorMessage { get; set; }

    public static SecureFileDownloadResult Error(string message) => new() { IsSuccess = false, ErrorMessage = message };
}
