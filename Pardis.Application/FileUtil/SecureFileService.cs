using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Pardis.Application.FileUtil;

/// <summary>
/// Secure file upload and download service with virus scanning and access control
/// </summary>
public class SecureFileService : ISecureFileService
{
    private readonly string _rootPath;
    private readonly ILogger<SecureFileService> _logger;
    private readonly IVirusScanService? _virusScanner;

    // Security constants
    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const int MaxFilenameLength = 255;
    private const int TokenValidityHours = 24;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".txt" };
    private static readonly Dictionary<string, string> AllowedFileTypes = new()
    {
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" },
        { "application/pdf", ".pdf" },
        { "application/msword", ".doc" },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
        { "text/plain", ".txt" }
    };

    public SecureFileService(string rootPath, ILogger<SecureFileService> logger, IVirusScanService? virusScanner = null)
    {
        _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _virusScanner = virusScanner;
    }

    public async Task<SecureFileUploadResult> SaveFileSecurely(IFormFile file, string category, string? userId = null)
    {
        if (file == null || file.Length == 0)
            return SecureFileUploadResult.Error("فایل انتخاب نشده است");

        try
        {
            // Validate file
            var validationResult = await ValidateFile(file);
            if (!validationResult.IsValid)
                return SecureFileUploadResult.Error(validationResult.ErrorMessage ?? "فایل نامعتبر است");

            // Virus scan if available
            if (_virusScanner != null)
            {
                var scanResult = await _virusScanner.ScanFileAsync(file);
                if (!scanResult.IsClean)
                {
                    _logger.LogWarning("Virus detected in file upload. UserId: {UserId}, FileName: {FileName}, Threat: {ThreatName}",
                        userId, file.FileName, scanResult.ThreatName);
                    return SecureFileUploadResult.Error("فایل آلوده شناسایی شد");
                }
            }

            // Generate secure filename and paths
            var secureFileName = GenerateSecureFileName(file.FileName);
            var sanitizedCategory = SanitizeDirectoryName(category);
            var uploadDirectory = Path.Combine(_rootPath, "uploads", sanitizedCategory);
            var fullPath = Path.Combine(uploadDirectory, secureFileName);

            // Ensure directory exists
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            // Save file with restricted access
            await SaveFileWithRestrictedAccess(file, fullPath);

            // Generate access token
            var accessToken = GenerateFileAccessToken(secureFileName, userId ?? "anonymous");

            return new SecureFileUploadResult
            {
                IsSuccess = true,
                SecureFileName = secureFileName,
                AccessToken = accessToken,
                Category = sanitizedCategory,
                OriginalFileName = file.FileName,
                FileSize = file.Length,
                ContentType = validationResult.ValidatedMimeType,
                UploadedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file securely. UserId: {UserId}, FileName: {FileName}",
                userId, file.FileName);
            return SecureFileUploadResult.Error("خطا در ذخیره فایل");
        }
    }

    public async Task<SecureFileDownloadResult> GetFileSecurely(string accessToken, string? userId = null)
    {
        try
        {
            var tokenValidation = ValidateFileAccessToken(accessToken, userId ?? "anonymous");
            if (!tokenValidation.IsValid)
                return SecureFileDownloadResult.Error("توکن دسترسی نامعتبر است");

            var filePath = Path.Combine(_rootPath, "uploads", tokenValidation.Category, tokenValidation.FileName);
            
            if (!File.Exists(filePath))
                return SecureFileDownloadResult.Error("فایل یافت نشد");

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var mimeType = GetMimeType(tokenValidation.FileName);

            return new SecureFileDownloadResult
            {
                IsSuccess = true,
                FileBytes = fileBytes,
                FileName = tokenValidation.OriginalFileName,
                ContentType = mimeType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file securely. Token: {Token}, UserId: {UserId}",
                accessToken, userId);
            return SecureFileDownloadResult.Error("خطا در دریافت فایل");
        }
    }

    private async Task<FileValidationResult> ValidateFile(IFormFile file)
    {
        // Size validation
        if (file.Length > MaxFileSize)
            return FileValidationResult.Invalid($"حجم فایل نباید بیشتر از {MaxFileSize / (1024 * 1024)} مگابایت باشد");

        // Filename validation
        if (string.IsNullOrEmpty(file.FileName) || file.FileName.Length > MaxFilenameLength)
            return FileValidationResult.Invalid("نام فایل نامعتبر است");

        // Extension validation
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return FileValidationResult.Invalid("نوع فایل مجاز نیست");

        // MIME type validation
        var detectedMimeType = await DetectMimeType(file);
        if (!AllowedFileTypes.ContainsKey(detectedMimeType))
            return FileValidationResult.Invalid("نوع فایل شناسایی شده مجاز نیست");

        return FileValidationResult.Valid(detectedMimeType);
    }

    private async Task<string> DetectMimeType(IFormFile file)
    {
        var buffer = new byte[512];
        using var stream = file.OpenReadStream();
        await stream.ReadAsync(buffer, 0, buffer.Length);
        stream.Position = 0;

        // Simple MIME type detection based on file signature
        if (buffer.Length >= 4)
        {
            // JPEG
            if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                return "image/jpeg";
            
            // PNG
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                return "image/png";
            
            // GIF
            if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46)
                return "image/gif";
            
            // PDF
            if (buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
                return "application/pdf";
        }

        // Fallback to file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedFileTypes.FirstOrDefault(x => x.Value == extension).Key ?? "application/octet-stream";
    }

    private string GenerateSecureFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var randomString = Convert.ToBase64String(randomBytes).Replace("/", "_").Replace("+", "-").TrimEnd('=');
        return $"{timestamp}_{randomString}{extension}";
    }

    private string SanitizeDirectoryName(string category)
    {
        if (string.IsNullOrEmpty(category))
            return "general";

        // Remove invalid characters and limit length
        var sanitized = string.Join("", category.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'));
        return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
    }

    private string GenerateFileAccessToken(string fileName, string userId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = $"{fileName}|{userId}|{timestamp}";
        
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("your-secret-key-here"));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(hash);
        
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{signature}"));
    }

    private FileAccessTokenValidation ValidateFileAccessToken(string token, string userId)
    {
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = decoded.Split('|');
            
            if (parts.Length != 4)
                return FileAccessTokenValidation.Invalid();

            var fileName = parts[0];
            var tokenUserId = parts[1];
            var timestamp = long.Parse(parts[2]);
            var providedSignature = parts[3];

            // Validate timestamp
            var tokenAge = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp;
            if (tokenAge > TokenValidityHours * 3600)
                return FileAccessTokenValidation.Invalid();

            // Validate user (allow anonymous access)
            if (tokenUserId != userId && tokenUserId != "anonymous")
                return FileAccessTokenValidation.Invalid();

            // Validate signature
            var payload = $"{fileName}|{tokenUserId}|{timestamp}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("your-secret-key-here"));
            var expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            
            if (providedSignature != expectedHash)
                return FileAccessTokenValidation.Invalid();

            return FileAccessTokenValidation.Valid(fileName, "uploads", fileName);
        }
        catch
        {
            return FileAccessTokenValidation.Invalid();
        }
    }

    private async Task SaveFileWithRestrictedAccess(IFormFile file, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(fileStream);
        
        // Set restrictive file permissions (Windows)
        if (OperatingSystem.IsWindows())
        {
            var fileInfo = new FileInfo(filePath);
            fileInfo.Attributes |= FileAttributes.ReadOnly;
        }
    }

    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedFileTypes.FirstOrDefault(x => x.Value == extension).Key ?? "application/octet-stream";
    }
}

// Supporting classes and interfaces

public interface IVirusScanService
{
    Task<VirusScanResult> ScanFileAsync(IFormFile file);
}

public class VirusScanResult
{
    public bool IsClean { get; set; }
    public string? ThreatName { get; set; }
}


public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string? ValidatedMimeType { get; set; }
    public string? ErrorMessage { get; set; }

    public static FileValidationResult Valid(string mimeType) => new() { IsValid = true, ValidatedMimeType = mimeType };
    public static FileValidationResult Invalid(string error) => new() { IsValid = false, ErrorMessage = error };
}

public class FileAccessTokenValidation
{
    public bool IsValid { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;

    public static FileAccessTokenValidation Valid(string fileName, string category, string originalFileName) => 
        new() { IsValid = true, FileName = fileName, Category = category, OriginalFileName = originalFileName };
    
    public static FileAccessTokenValidation Invalid() => new() { IsValid = false };
}
