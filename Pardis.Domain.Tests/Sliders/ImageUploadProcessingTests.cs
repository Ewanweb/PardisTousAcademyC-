using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pardis.Application.Sliders._Shared;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for image upload processing
    /// Feature: simplified-slider-system, Property 3: Image Upload Processing
    /// Validates: Requirements 3.1, 3.2
    /// </summary>
    public class ImageUploadProcessingTests
    {
        private readonly SliderImageService _imageService;

        public ImageUploadProcessingTests()
        {
            _imageService = new SliderImageService();
        }

        [Property(MaxTest = 100)]
        public async Task<bool> HandleImageUploadAsync_Should_Store_File_And_Generate_Valid_URL_For_Valid_Images(
            NonEmptyString fileName, PositiveInt fileSize)
        {
            // Arrange - create valid image file
            var validFileName = $"{fileName.Get}.jpg";
            var validFileSize = Math.Min(fileSize.Get, 1024 * 1024); // Max 1MB for test performance
            var imageFile = TestFileUtilities.CreateTestFile(validFileName, validFileSize, "image/jpeg");

            try
            {
                // Act - upload the image
                var resultUrl = await _imageService.HandleImageUploadAsync(imageFile);

                // Assert - verify URL is generated and accessible
                var isValidUrl = !string.IsNullOrWhiteSpace(resultUrl) && 
                               resultUrl.StartsWith("/uploads/sliders/") &&
                               resultUrl.EndsWith(".jpg");

                // Verify file was actually created
                var fileName_extracted = Path.GetFileName(resultUrl);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/sliders", fileName_extracted);
                var fileExists = File.Exists(filePath);

                // Cleanup - delete the test file
                if (fileExists)
                {
                    await _imageService.DeleteImageAsync(resultUrl);
                }

                return isValidUrl && fileExists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public async Task<bool> HandleImageUploadAsync_Should_Generate_Unique_URLs_For_Same_Filename(
            NonEmptyString fileName)
        {
            // Arrange - create two identical image files
            var validFileName = $"{fileName.Get}.png";
            var imageFile1 = TestFileUtilities.CreateTestFile(validFileName, 1024, "image/png");
            var imageFile2 = TestFileUtilities.CreateTestFile(validFileName, 1024, "image/png");

            try
            {
                // Act - upload both images
                var url1 = await _imageService.HandleImageUploadAsync(imageFile1);
                var url2 = await _imageService.HandleImageUploadAsync(imageFile2);

                // Assert - URLs should be different (unique)
                var urlsAreDifferent = !string.Equals(url1, url2, StringComparison.OrdinalIgnoreCase);

                // Both URLs should be valid
                var url1IsValid = !string.IsNullOrWhiteSpace(url1) && url1.StartsWith("/uploads/sliders/");
                var url2IsValid = !string.IsNullOrWhiteSpace(url2) && url2.StartsWith("/uploads/sliders/");

                // Cleanup
                await _imageService.DeleteImageAsync(url1);
                await _imageService.DeleteImageAsync(url2);

                return urlsAreDifferent && url1IsValid && url2IsValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public bool GenerateUniqueFileName_Should_Always_Generate_Different_Names_For_Same_Input(
            NonEmptyString originalFileName)
        {
            // Arrange
            var fileName = $"{originalFileName.Get}.jpg";

            try
            {
                // Act - generate multiple unique filenames
                var name1 = _imageService.GenerateUniqueFileName(fileName);
                var name2 = _imageService.GenerateUniqueFileName(fileName);
                var name3 = _imageService.GenerateUniqueFileName(fileName);

                // Assert - all names should be different
                var allDifferent = name1 != name2 && name2 != name3 && name1 != name3;

                // All names should contain the original extension
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var allHaveCorrectExtension = name1.EndsWith(extension) && 
                                            name2.EndsWith(extension) && 
                                            name3.EndsWith(extension);

                // All names should start with "slider_"
                var allHavePrefix = name1.StartsWith("slider_") && 
                                  name2.StartsWith("slider_") && 
                                  name3.StartsWith("slider_");

                return allDifferent && allHaveCorrectExtension && allHavePrefix;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public async Task<bool> DeleteImageAsync_Should_Remove_File_When_Valid_URL_Provided(
            NonEmptyString fileName)
        {
            // Arrange - create and upload an image first
            var validFileName = $"{fileName.Get}.webp";
            var imageFile = TestFileUtilities.CreateTestFile(validFileName, 1024, "image/webp");

            try
            {
                // Upload the image
                var imageUrl = await _imageService.HandleImageUploadAsync(imageFile);
                
                // Verify file exists
                var fileNameExtracted = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/sliders", fileNameExtracted);
                var fileExistsBeforeDelete = File.Exists(filePath);

                // Act - delete the image
                await _imageService.DeleteImageAsync(imageUrl);

                // Assert - file should no longer exist
                var fileExistsAfterDelete = File.Exists(filePath);

                return fileExistsBeforeDelete && !fileExistsAfterDelete;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public async Task<bool> DeleteImageAsync_Should_Handle_Invalid_URLs_Gracefully(string invalidUrl)
        {
            try
            {
                // Act - attempt to delete with invalid URL
                await _imageService.DeleteImageAsync(invalidUrl);

                // Assert - should not throw exception
                return true;
            }
            catch (Exception)
            {
                // Should not throw exceptions for invalid URLs
                return false;
            }
        }

        [Fact]
        public async Task HandleImageUploadAsync_Should_Create_Directory_If_Not_Exists()
        {
            // Arrange - ensure directory doesn't exist
            var testDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/sliders");
            if (Directory.Exists(testDirectoryPath))
            {
                // Temporarily rename it
                var tempPath = testDirectoryPath + "_temp";
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
                Directory.Move(testDirectoryPath, tempPath);
            }

            var imageFile = TestFileUtilities.CreateTestFile("test.jpg", 1024, "image/jpeg");

            try
            {
                // Act - upload image (should create directory)
                var resultUrl = await _imageService.HandleImageUploadAsync(imageFile);

                // Assert - directory should now exist
                var directoryExists = Directory.Exists(testDirectoryPath);
                var fileExists = !string.IsNullOrWhiteSpace(resultUrl);

                Assert.True(directoryExists, "Directory should be created if it doesn't exist");
                Assert.True(fileExists, "File should be uploaded successfully");

                // Cleanup
                if (fileExists)
                    await _imageService.DeleteImageAsync(resultUrl);
            }
            finally
            {
                // Restore original directory if it existed
                var tempPath = testDirectoryPath + "_temp";
                if (Directory.Exists(tempPath))
                {
                    if (Directory.Exists(testDirectoryPath))
                        Directory.Delete(testDirectoryPath, true);
                    Directory.Move(tempPath, testDirectoryPath);
                }
            }
        }
    }
}