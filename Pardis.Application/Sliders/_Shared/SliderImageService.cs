using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared.Exceptions;

namespace Pardis.Application.Sliders._Shared;

public class SliderImageService : ISliderImageService
{
    private readonly string[] _allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private const string SliderImagesDirectory = "wwwroot/uploads/sliders";

    public async Task<string> HandleImageUploadAsync(IFormFile imageFile)
    {
        if (imageFile == null)
            throw new ArgumentNullException(nameof(imageFile), "Image file cannot be null");

        // Validate the image file
        ValidateImageFile(imageFile);

        // Generate unique filename
        var uniqueFileName = GenerateUniqueFileName(imageFile.FileName);

        // Ensure directory exists
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), SliderImagesDirectory);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        // Save the file
        var filePath = Path.Combine(directoryPath, uniqueFileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        // Return accessible URL
        return $"/uploads/sliders/{uniqueFileName}";
    }

    public bool ValidateImageFile(IFormFile imageFile)
    {
        if (imageFile == null)
            throw new ArgumentNullException(nameof(imageFile), "Image file cannot be null");

        // Validate file type
        if (!_allowedImageTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
        {
            throw new ValidationException($"نوع فایل مجاز نیست. انواع مجاز: {string.Join(", ", _allowedImageTypes)}");
        }

        // Validate file size
        if (imageFile.Length > MaxFileSizeBytes)
        {
            throw new ValidationException($"حجم فایل بیش از حد مجاز است. حداکثر حجم مجاز: {MaxFileSizeBytes / (1024 * 1024)} مگابایت");
        }

        // Validate file has content
        if (imageFile.Length == 0)
        {
            throw new ValidationException("فایل خالی است");
        }

        // Validate filename
        if (string.IsNullOrWhiteSpace(imageFile.FileName))
        {
            throw new ValidationException("نام فایل نامعتبر است");
        }

        return true;
    }

    public string GenerateUniqueFileName(string originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException("Original filename cannot be null or empty", nameof(originalFileName));

        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg"; // Default extension

        // Generate unique filename using GUID and timestamp
        var uniqueId = Guid.NewGuid().ToString("N");
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        return $"slider_{timestamp}_{uniqueId}{extension.ToLowerInvariant()}";
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        try
        {
            // Extract filename from URL
            var fileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            // Construct full file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), SliderImagesDirectory, fileName);
            
            // Delete file if it exists
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
        catch (Exception)
        {
            // Log error but don't throw - deletion failure shouldn't break the application
            // In a real application, you would log this error
        }
    }
}