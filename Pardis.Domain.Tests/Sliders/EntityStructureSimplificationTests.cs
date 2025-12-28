using System;
using System.Reflection;
using FsCheck;
using FsCheck.Xunit;
using Pardis.Domain.Sliders;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for entity structure simplification
    /// Feature: simplified-slider-system, Property 1: Entity Structure Simplification
    /// Validates: Requirements 1.1, 1.2, 2.1, 2.2, 6.1
    /// </summary>
    public class EntityStructureSimplificationTests
    {
        private readonly string[] _expectedCoreFields = 
        {
            "Title", "Description", "ImageUrl", "ActionLabel", "ActionLink", "Order", "IsActive", "CreatedByUserId"
        };

        private readonly string[] _forbiddenComplexFields = 
        {
            "Badge", "PrimaryActionLabel", "PrimaryActionLink", "SecondaryActionLabel", "SecondaryActionLink",
            "StatsJson", "IsPermanent", "ExpiresAt", "Subtitle", "Type", "StudentName", "CourseName", 
            "Duration", "CourseId", "LinkUrl", "ButtonText"
        };

        [Property(MaxTest = 100)]
        public bool HeroSlide_Should_Have_Only_Core_Fields_And_No_Complex_Fields(string title, string imageUrl, Guid userId)
        {
            // Arrange - ensure valid inputs
            var validTitle = string.IsNullOrWhiteSpace(title) ? "Test Title" : title.Trim();
            var validImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? "https://example.com/image.jpg" : imageUrl.Trim();
            var validUserId = userId == Guid.Empty ? Guid.NewGuid() : userId;

            // Act - create entity
            var heroSlide = HeroSlide.Create(validTitle, validImageUrl, validUserId);

            // Assert - verify structure
            var entityType = typeof(HeroSlide);
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Check that all expected core fields are present
            var hasCoreFields = Array.TrueForAll(_expectedCoreFields, fieldName =>
                Array.Exists(properties, prop => prop.Name == fieldName));

            // Check that no forbidden complex fields are present
            var hasNoComplexFields = Array.TrueForAll(_forbiddenComplexFields, fieldName =>
                !Array.Exists(properties, prop => prop.Name == fieldName));

            // Verify entity has exactly the core fields (plus inherited BaseEntity fields)
            var baseEntityFields = typeof(BaseEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var expectedTotalFields = _expectedCoreFields.Length + baseEntityFields.Length;
            var actualFieldCount = properties.Length;

            return hasCoreFields && hasNoComplexFields && actualFieldCount == expectedTotalFields;
        }

        [Property(MaxTest = 100)]
        public bool SuccessStory_Should_Have_Only_Core_Fields_And_No_Complex_Fields(string title, string imageUrl, Guid userId)
        {
            // Arrange - ensure valid inputs
            var validTitle = string.IsNullOrWhiteSpace(title) ? "Test Title" : title.Trim();
            var validImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? "https://example.com/image.jpg" : imageUrl.Trim();
            var validUserId = userId == Guid.Empty ? Guid.NewGuid() : userId;

            // Act - create entity
            var successStory = SuccessStory.Create(validTitle, validImageUrl, validUserId);

            // Assert - verify structure
            var entityType = typeof(SuccessStory);
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Check that all expected core fields are present
            var hasCoreFields = Array.TrueForAll(_expectedCoreFields, fieldName =>
                Array.Exists(properties, prop => prop.Name == fieldName));

            // Check that no forbidden complex fields are present
            var hasNoComplexFields = Array.TrueForAll(_forbiddenComplexFields, fieldName =>
                !Array.Exists(properties, prop => prop.Name == fieldName));

            // Verify entity has exactly the core fields (plus inherited BaseEntity fields)
            var baseEntityFields = typeof(BaseEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var expectedTotalFields = _expectedCoreFields.Length + baseEntityFields.Length;
            var actualFieldCount = properties.Length;

            return hasCoreFields && hasNoComplexFields && actualFieldCount == expectedTotalFields;
        }

        [Property(MaxTest = 100)]
        public bool HeroSlide_And_SuccessStory_Should_Have_Identical_Structure(string title, string imageUrl, Guid userId)
        {
            // Arrange - ensure valid inputs
            var validTitle = string.IsNullOrWhiteSpace(title) ? "Test Title" : title.Trim();
            var validImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? "https://example.com/image.jpg" : imageUrl.Trim();
            var validUserId = userId == Guid.Empty ? Guid.NewGuid() : userId;

            // Act - get both entity types
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            var heroSlideProperties = heroSlideType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var successStoryProperties = successStoryType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Assert - verify identical structure
            if (heroSlideProperties.Length != successStoryProperties.Length)
                return false;

            // Check that both have the same property names and types
            foreach (var heroProperty in heroSlideProperties)
            {
                var matchingProperty = Array.Find(successStoryProperties, 
                    prop => prop.Name == heroProperty.Name && prop.PropertyType == heroProperty.PropertyType);
                
                if (matchingProperty == null)
                    return false;
            }

            return true;
        }

        [Fact]
        public void HeroSlide_Should_Not_Have_Expiration_Methods()
        {
            // Arrange
            var entityType = typeof(HeroSlide);

            // Act & Assert - verify no expiration-related methods exist
            var isExpiredMethod = entityType.GetMethod("IsExpired");
            var getTimeRemainingMethod = entityType.GetMethod("GetTimeRemaining");

            Assert.Null(isExpiredMethod);
            Assert.Null(getTimeRemainingMethod);
        }

        [Fact]
        public void SuccessStory_Should_Not_Have_Expiration_Methods()
        {
            // Arrange
            var entityType = typeof(SuccessStory);

            // Act & Assert - verify no expiration-related methods exist
            var isExpiredMethod = entityType.GetMethod("IsExpired");
            var getTimeRemainingMethod = entityType.GetMethod("GetTimeRemaining");

            Assert.Null(isExpiredMethod);
            Assert.Null(getTimeRemainingMethod);
        }
    }
}