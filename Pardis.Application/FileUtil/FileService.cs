using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Pardis.Application.FileUtil;

/// <summary>
/// DEPRECATED: Use ISecureFileService instead
/// Legacy file service with security vulnerabilities
/// </summary>
[Obsolete("Use ISecureFileService for secure file operations")]
public class FileService : IFileService
{
    private readonly string _rootPath;
    private readonly ILogger<FileService> _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _rootPath = configuration["FileStorage:RootPath"] ?? Directory.GetCurrentDirectory();
        _logger = logger;
    }

    public void DeleteDirectory(string directoryPath)
    {
        try
        {
            var fullPath = Path.Combine(_rootPath, directoryPath);
            
            // CRITICAL: Validate path is within allowed directory
            var normalizedPath = Path.GetFullPath(fullPath);
            var normalizedRoot = Path.GetFullPath(_rootPath);
            
            if (!normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Path traversal attempt detected in DeleteDirectory: {Path}", directoryPath);
                throw new UnauthorizedAccessException("Invalid directory path");
            }

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                _logger.LogInformation("Directory deleted: {Path}", directoryPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting directory: {Path}", directoryPath);
            throw;
        }
    }

    public void DeleteFile(string path, string fileName)
    {
        try
        {
            var filePath = Path.Combine(_rootPath, path, fileName);
            
            // CRITICAL: Validate path is within allowed directory
            var normalizedPath = Path.GetFullPath(filePath);
            var normalizedRoot = Path.GetFullPath(_rootPath);
            
            if (!normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Path traversal attempt detected in DeleteFile: {Path}/{FileName}", path, fileName);
                throw new UnauthorizedAccessException("Invalid file path");
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {Path}/{FileName}", path, fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}/{FileName}", path, fileName);
            throw;
        }
    }

    public void DeleteFile(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_rootPath, filePath);
            
            // CRITICAL: Validate path is within allowed directory
            var normalizedPath = Path.GetFullPath(fullPath);
            var normalizedRoot = Path.GetFullPath(_rootPath);
            
            if (!normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Path traversal attempt detected in DeleteFile: {Path}", filePath);
                throw new UnauthorizedAccessException("Invalid file path");
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted: {Path}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", filePath);
            throw;
        }
    }

    [Obsolete("Use ISecureFileService.SaveFileSecurely instead")]
    public async Task SaveFile(IFormFile file, string directoryPath)
    {
        _logger.LogWarning("SECURITY WARNING: Using deprecated SaveFile method. Migrate to ISecureFileService");
        
        if (file == null)
            throw new InvalidDataException("file is Null");

        // CRITICAL: This method has security vulnerabilities and should not be used
        throw new NotSupportedException("This method is deprecated due to security vulnerabilities. Use ISecureFileService.SaveFileSecurely instead");
    }

    [Obsolete("Use ISecureFileService.SaveFileSecurely instead")]
    public async Task<string> SaveFileAndGenerateName(IFormFile? file, string directoryPath)
    {
        _logger.LogWarning("SECURITY WARNING: Using deprecated SaveFileAndGenerateName method. Migrate to ISecureFileService");
        
        if (file == null)
            throw new InvalidDataException("file is Null");

        // CRITICAL: This method has security vulnerabilities and should not be used
        throw new NotSupportedException("This method is deprecated due to security vulnerabilities. Use ISecureFileService.SaveFileSecurely instead");
    }
}