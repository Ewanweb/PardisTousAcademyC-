using Microsoft.AspNetCore.Http;

namespace Pardis.Application.Sliders._Shared;

public interface ISliderImageService
{
    /// <summary>
    /// Handles image upload with validation and secure storage
    /// </summary>
    /// <param name="imageFile">The image file to upload</param>
    /// <returns>The accessible URL for the uploaded image</returns>
    Task<string> HandleImageUploadAsync(IFormFile imageFile);

    /// <summary>
    /// Validates image file type and size
    /// </summary>
    /// <param name="imageFile">The image file to validate</param>
    /// <returns>True if valid, throws exception if invalid</returns>
    bool ValidateImageFile(IFormFile imageFile);

    /// <summary>
    /// Generates a unique filename for the uploaded image
    /// </summary>
    /// <param name="originalFileName">The original filename</param>
    /// <returns>A unique filename</returns>
    string GenerateUniqueFileName(string originalFileName);

    /// <summary>
    /// Deletes an image file from storage
    /// </summary>
    /// <param name="imageUrl">The URL of the image to delete</param>
    Task DeleteImageAsync(string imageUrl);
}