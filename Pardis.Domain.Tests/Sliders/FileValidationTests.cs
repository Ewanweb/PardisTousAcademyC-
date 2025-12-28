using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pardis.Application.Sliders._Shared;
using Pardis.Application._Shared.Exceptions;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for file validation
    /// Feature: simplified-slider-system, Property 4: File Validation
    /// Validates: Requirements 3.5
    /// </summary>
    public class FileValidationTests
    {
        private readonly SliderImageService _imageService;
        private readonly string[] _validImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        private readonly string[] _invalidImageTypes = { "text/plain", "application/pdf", "video/mp4", "audio/mp3", "image/gif", "image/bmp" };

        public FileValidationTests()
        {
            _imageService = new SliderImageService();
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Accept_Only_Valid_Image_Types(NonEmptyString fileName)
        {
            // Test valid image types
            foreach (var validType in _validImageTypes)
            {
                var validFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", 1024, validType);
                
                try
                {
                    var result = _imageService.ValidateImageFile(validFile);
                    if (!result) return false; // Should return true for valid types
                }
                catch (ValidationException)
                {
                    return false; // Should not throw for valid types
                }
            }

            // Test invalid image types
            foreach (var invalidType in _invalidImageTypes)
            {
                var invalidFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.txt", 1024, invalidType);
                
                try
                {
                    _imageService.ValidateImageFile(invalidFile);
                    return false; // Should throw exception for invalid types
                }
                catch (ValidationException)
                {
                    // Expected behavior for invalid types
                    continue;
                }
                catch (Exception)
                {
                    return false; // Should throw ValidationException specifically
                }
            }

            return true;
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Reject_Files_Exceeding_Size_Limit(NonEmptyString fileName)
        {
            // Arrange - create file larger than 5MB limit
            var oversizedFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", 6 * 1024 * 1024, "image/jpeg"); // 6MB

            try
            {
                // Act - validate oversized file
                _imageService.ValidateImageFile(oversizedFile);
                
                // Assert - should have thrown exception
                return false;
            }
            catch (ValidationException ex)
            {
                // Expected behavior - should reject oversized files
                return ex.Message.Contains("حجم فایل بیش از حد مجاز است");
            }
            catch (Exception)
            {
                // Should throw ValidationException specifically
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Accept_Files_Within_Size_Limit(NonEmptyString fileName, PositiveInt fileSize)
        {
            // Arrange - create file within size limit (max 5MB)
            var validSize = Math.Min(fileSize.Get, 5 * 1024 * 1024 - 1); // Just under 5MB
            var validFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", validSize, "image/jpeg");

            try
            {
                // Act - validate file within size limit
                var result = _imageService.ValidateImageFile(validFile);
                
                // Assert - should accept valid sized files
                return result;
            }
            catch (ValidationException)
            {
                // Should not throw for valid sized files
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Reject_Empty_Files(NonEmptyString fileName)
        {
            // Arrange - create empty file
            var emptyFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", 0, "image/jpeg");

            try
            {
                // Act - validate empty file
                _imageService.ValidateImageFile(emptyFile);
                
                // Assert - should have thrown exception
                return false;
            }
            catch (ValidationException ex)
            {
                // Expected behavior - should reject empty files
                return ex.Message.Contains("فایل خالی است");
            }
            catch (Exception)
            {
                // Should throw ValidationException specifically
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Reject_Files_With_Invalid_Filenames(string invalidFileName)
        {
            // Test null, empty, or whitespace filenames
            var testFilenames = new[] { null, "", "   ", "\t", "\n" };
            
            foreach (var filename in testFilenames)
            {
                var invalidFile = TestFileUtilities.CreateTestFile(filename, 1024, "image/jpeg");

                try
                {
                    // Act - validate file with invalid filename
                    _imageService.ValidateImageFile(invalidFile);
                    
                    // Assert - should have thrown exception
                    return false;
                }
                catch (ValidationException ex)
                {
                    // Expected behavior - should reject invalid filenames
                    if (!ex.Message.Contains("نام فایل نامعتبر است"))
                        return false;
                }
                catch (Exception)
                {
                    // Should throw ValidationException specifically
                    return false;
                }
            }

            return true;
        }

        [Fact]
        public void ValidateImageFile_Should_Throw_ArgumentNullException_For_Null_File()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _imageService.ValidateImageFile(null));
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Handle_Case_Insensitive_Content_Types(NonEmptyString fileName)
        {
            // Test various case combinations of valid content types
            var caseVariations = new[]
            {
                "IMAGE/JPEG", "Image/Jpeg", "image/JPEG",
                "IMAGE/PNG", "Image/Png", "image/PNG",
                "IMAGE/WEBP", "Image/Webp", "image/WEBP"
            };

            foreach (var contentType in caseVariations)
            {
                var file = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", 1024, contentType);

                try
                {
                    var result = _imageService.ValidateImageFile(file);
                    if (!result) return false; // Should accept case variations
                }
                catch (ValidationException)
                {
                    return false; // Should not throw for valid types regardless of case
                }
            }

            return true;
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Provide_Descriptive_Error_Messages_For_Invalid_Types(NonEmptyString fileName)
        {
            // Arrange - create file with invalid type
            var invalidFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.pdf", 1024, "application/pdf");

            try
            {
                // Act - validate invalid file type
                _imageService.ValidateImageFile(invalidFile);
                
                // Assert - should have thrown exception
                return false;
            }
            catch (ValidationException ex)
            {
                // Expected behavior - should provide descriptive error message
                var hasDescriptiveMessage = ex.Message.Contains("نوع فایل مجاز نیست") &&
                                          ex.Message.Contains("image/jpeg") &&
                                          ex.Message.Contains("image/png") &&
                                          ex.Message.Contains("image/webp");
                
                return hasDescriptiveMessage;
            }
            catch (Exception)
            {
                // Should throw ValidationException specifically
                return false;
            }
        }

        [Property(MaxTest = 100)]
        public bool ValidateImageFile_Should_Provide_Descriptive_Error_Messages_For_Oversized_Files(NonEmptyString fileName)
        {
            // Arrange - create oversized file
            var oversizedFile = TestFileUtilities.CreateTestFile($"{fileName.Get}.jpg", 6 * 1024 * 1024, "image/jpeg");

            try
            {
                // Act - validate oversized file
                _imageService.ValidateImageFile(oversizedFile);
                
                // Assert - should have thrown exception
                return false;
            }
            catch (ValidationException ex)
            {
                // Expected behavior - should provide descriptive error message with size limit
                var hasDescriptiveMessage = ex.Message.Contains("حجم فایل بیش از حد مجاز است") &&
                                          ex.Message.Contains("5") && // Should mention 5MB limit
                                          ex.Message.Contains("مگابایت");
                
                return hasDescriptiveMessage;
            }
            catch (Exception)
            {
                // Should throw ValidationException specifically
                return false;
            }
        }
    }
}