using System;
using System.IO;
using System.Threading.Tasks;
using Pardis.Application.Sliders._Shared;
using Pardis.Application._Shared.Exceptions;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Unit tests for SliderImageService to verify basic functionality
    /// </summary>
    public class ImageServiceUnitTests
    {
        private readonly SliderImageService _imageService;

        public ImageServiceUnitTests()
        {
            _imageService = new SliderImageService();
        }

        [Fact]
        public void GenerateUniqueFileName_Should_Generate_Different_Names()
        {
            // Arrange
            var originalFileName = "test.jpg";

            // Act
            var name1 = _imageService.GenerateUniqueFileName(originalFileName);
            var name2 = _imageService.GenerateUniqueFileName(originalFileName);

            // Assert
            Assert.NotEqual(name1, name2);
            Assert.StartsWith("slider_", name1);
            Assert.StartsWith("slider_", name2);
            Assert.EndsWith(".jpg", name1);
            Assert.EndsWith(".jpg", name2);
        }

        [Fact]
        public void GenerateUniqueFileName_Should_Throw_For_Null_Filename()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _imageService.GenerateUniqueFileName(null));
        }

        [Fact]
        public void GenerateUniqueFileName_Should_Throw_For_Empty_Filename()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _imageService.GenerateUniqueFileName(""));
        }

        [Fact]
        public void ValidateImageFile_Should_Throw_For_Null_File()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _imageService.ValidateImageFile(null));
        }

        [Fact]
        public void ValidateImageFile_Should_Accept_Valid_JPEG()
        {
            // Arrange
            var validFile = TestFileUtilities.CreateTestFile("test.jpg", 1024, "image/jpeg");

            // Act & Assert
            var result = _imageService.ValidateImageFile(validFile);
            Assert.True(result);
        }

        [Fact]
        public void ValidateImageFile_Should_Accept_Valid_PNG()
        {
            // Arrange
            var validFile = TestFileUtilities.CreateTestFile("test.png", 1024, "image/png");

            // Act & Assert
            var result = _imageService.ValidateImageFile(validFile);
            Assert.True(result);
        }

        [Fact]
        public void ValidateImageFile_Should_Accept_Valid_WebP()
        {
            // Arrange
            var validFile = TestFileUtilities.CreateTestFile("test.webp", 1024, "image/webp");

            // Act & Assert
            var result = _imageService.ValidateImageFile(validFile);
            Assert.True(result);
        }

        [Fact]
        public void ValidateImageFile_Should_Reject_Invalid_Type()
        {
            // Arrange
            var invalidFile = TestFileUtilities.CreateTestFile("test.pdf", 1024, "application/pdf");

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _imageService.ValidateImageFile(invalidFile));
            Assert.Contains("نوع فایل مجاز نیست", exception.Message);
        }

        [Fact]
        public void ValidateImageFile_Should_Reject_Oversized_File()
        {
            // Arrange - create 6MB file (over 5MB limit)
            var oversizedFile = TestFileUtilities.CreateTestFile("test.jpg", 6 * 1024 * 1024, "image/jpeg");

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _imageService.ValidateImageFile(oversizedFile));
            Assert.Contains("حجم فایل بیش از حد مجاز است", exception.Message);
        }

        [Fact]
        public void ValidateImageFile_Should_Reject_Empty_File()
        {
            // Arrange
            var emptyFile = TestFileUtilities.CreateTestFile("test.jpg", 0, "image/jpeg");

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _imageService.ValidateImageFile(emptyFile));
            Assert.Contains("فایل خالی است", exception.Message);
        }

        [Fact]
        public async Task DeleteImageAsync_Should_Not_Throw_For_Invalid_URL()
        {
            // Act & Assert - should not throw
            await _imageService.DeleteImageAsync("invalid-url");
            await _imageService.DeleteImageAsync(null);
            await _imageService.DeleteImageAsync("");
        }
    }
}