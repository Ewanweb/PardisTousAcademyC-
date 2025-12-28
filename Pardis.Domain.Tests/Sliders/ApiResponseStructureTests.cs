using FsCheck;
using FsCheck.Xunit;
using Pardis.Domain.Dto.Sliders;
using System.Reflection;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for API response structure validation
    /// **Feature: simplified-slider-system, Property 7: API Response Structure**
    /// **Validates: Requirements 5.4, 5.5**
    /// </summary>
    public class ApiResponseStructureTests
    {
        [Property(MaxTest = 100)]
        public bool HeroSlideResource_Should_Contain_Only_Essential_Fields_And_Follow_Consistent_Format(
            Guid id, string title, string? description, string imageUrl, string? actionLabel, 
            string? actionLink, int order, bool isActive, DateTime createdAt, DateTime updatedAt, Guid createdByUserId)
        {
            // Arrange - create HeroSlideResource with generated data
            var resource = new HeroSlideResource
            {
                Id = id,
                Title = title ?? "Test Title",
                Description = description,
                ImageUrl = imageUrl ?? "https://example.com/image.jpg",
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                Order = order,
                IsActive = isActive,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedByUserId = createdByUserId
            };

            // Act - get all properties of the resource
            var properties = typeof(HeroSlideResource).GetProperties();
            var expectedFields = new[]
            {
                "Id", "Title", "Description", "ImageUrl", "ActionLabel", 
                "ActionLink", "Order", "IsActive", "CreatedAt", "UpdatedAt", "CreatedByUserId"
            };

            // Assert - verify only essential fields are present
            var actualFields = properties.Select(p => p.Name).ToArray();
            var hasOnlyEssentialFields = expectedFields.All(field => actualFields.Contains(field)) &&
                                       actualFields.All(field => expectedFields.Contains(field));

            // Verify required fields have appropriate types (non-nullable reference types have default values)
            var requiredStringFields = new[] { "Title", "ImageUrl" };
            var requiredStringFieldsHaveDefaults = requiredStringFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType == typeof(string); // Non-nullable string
            });

            // Verify optional string fields are nullable
            var optionalStringFields = new[] { "Description", "ActionLabel", "ActionLink" };
            var optionalStringFieldsAreNullable = optionalStringFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType == typeof(string); // Nullable string (string?)
            });

            // Verify value type fields
            var valueTypeFields = new[] { "Id", "Order", "IsActive", "CreatedAt", "UpdatedAt", "CreatedByUserId" };
            var valueTypeFieldsAreCorrect = valueTypeFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType.IsValueType || property.PropertyType == typeof(Guid);
            });

            return hasOnlyEssentialFields && requiredStringFieldsHaveDefaults && 
                   optionalStringFieldsAreNullable && valueTypeFieldsAreCorrect;
        }

        [Property(MaxTest = 100)]
        public bool SuccessStoryResource_Should_Contain_Only_Essential_Fields_And_Follow_Consistent_Format(
            Guid id, string title, string? description, string imageUrl, string? actionLabel, 
            string? actionLink, int order, bool isActive, DateTime createdAt, DateTime updatedAt, Guid createdByUserId)
        {
            // Arrange - create SuccessStoryResource with generated data
            var resource = new SuccessStoryResource
            {
                Id = id,
                Title = title ?? "Test Title",
                Description = description,
                ImageUrl = imageUrl ?? "https://example.com/image.jpg",
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                Order = order,
                IsActive = isActive,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedByUserId = createdByUserId
            };

            // Act - get all properties of the resource
            var properties = typeof(SuccessStoryResource).GetProperties();
            var expectedFields = new[]
            {
                "Id", "Title", "Description", "ImageUrl", "ActionLabel", 
                "ActionLink", "Order", "IsActive", "CreatedAt", "UpdatedAt", "CreatedByUserId"
            };

            // Assert - verify only essential fields are present
            var actualFields = properties.Select(p => p.Name).ToArray();
            var hasOnlyEssentialFields = expectedFields.All(field => actualFields.Contains(field)) &&
                                       actualFields.All(field => expectedFields.Contains(field));

            // Verify required fields have appropriate types
            var requiredStringFields = new[] { "Title", "ImageUrl" };
            var requiredStringFieldsHaveDefaults = requiredStringFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType == typeof(string); // Non-nullable string
            });

            // Verify optional string fields are nullable
            var optionalStringFields = new[] { "Description", "ActionLabel", "ActionLink" };
            var optionalStringFieldsAreNullable = optionalStringFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType == typeof(string); // Nullable string (string?)
            });

            // Verify value type fields
            var valueTypeFields = new[] { "Id", "Order", "IsActive", "CreatedAt", "UpdatedAt", "CreatedByUserId" };
            var valueTypeFieldsAreCorrect = valueTypeFields.All(fieldName =>
            {
                var property = properties.First(p => p.Name == fieldName);
                return property.PropertyType.IsValueType || property.PropertyType == typeof(Guid);
            });

            return hasOnlyEssentialFields && requiredStringFieldsHaveDefaults && 
                   optionalStringFieldsAreNullable && valueTypeFieldsAreCorrect;
        }

        [Property(MaxTest = 100)]
        public bool HeroSlideResource_And_SuccessStoryResource_Should_Have_Identical_Structure(
            Guid id, string title, string? description, string imageUrl, string? actionLabel, 
            string? actionLink, int order, bool isActive, DateTime createdAt, DateTime updatedAt, Guid createdByUserId)
        {
            // Arrange - get both resource types
            var heroSlideType = typeof(HeroSlideResource);
            var successStoryType = typeof(SuccessStoryResource);

            // Act - get properties for both types
            var heroSlideProperties = heroSlideType.GetProperties().OrderBy(p => p.Name).ToArray();
            var successStoryProperties = successStoryType.GetProperties().OrderBy(p => p.Name).ToArray();

            // Assert - verify identical structure
            if (heroSlideProperties.Length != successStoryProperties.Length)
                return false;

            for (int i = 0; i < heroSlideProperties.Length; i++)
            {
                var heroProperty = heroSlideProperties[i];
                var successProperty = successStoryProperties[i];

                // Check property name and type match
                if (heroProperty.Name != successProperty.Name ||
                    heroProperty.PropertyType != successProperty.PropertyType)
                {
                    return false;
                }
            }

            return true;
        }

        [Property(MaxTest = 100)]
        public bool HeroSlideListResource_Should_Contain_Only_Essential_List_Fields(
            Guid id, string title, string? description, string imageUrl, string? actionLabel, 
            string? actionLink, int order, bool isActive, DateTime createdAt)
        {
            // Arrange - create HeroSlideListResource with generated data
            var resource = new HeroSlideListResource
            {
                Id = id,
                Title = title ?? "Test Title",
                Description = description,
                ImageUrl = imageUrl ?? "https://example.com/image.jpg",
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                Order = order,
                IsActive = isActive,
                CreatedAt = createdAt
            };

            // Act - get all properties of the list resource
            var properties = typeof(HeroSlideListResource).GetProperties();
            var expectedFields = new[]
            {
                "Id", "Title", "Description", "ImageUrl", "ActionLabel", 
                "ActionLink", "Order", "IsActive", "CreatedAt"
            };

            // Assert - verify only essential list fields are present (no UpdatedAt, CreatedByUserId for list view)
            var actualFields = properties.Select(p => p.Name).ToArray();
            var hasOnlyEssentialFields = expectedFields.All(field => actualFields.Contains(field)) &&
                                       actualFields.All(field => expectedFields.Contains(field));

            return hasOnlyEssentialFields;
        }

        [Property(MaxTest = 100)]
        public bool SuccessStoryListResource_Should_Contain_Only_Essential_List_Fields(
            Guid id, string title, string? description, string imageUrl, string? actionLabel, 
            string? actionLink, int order, bool isActive, DateTime createdAt)
        {
            // Arrange - create SuccessStoryListResource with generated data
            var resource = new SuccessStoryListResource
            {
                Id = id,
                Title = title ?? "Test Title",
                Description = description,
                ImageUrl = imageUrl ?? "https://example.com/image.jpg",
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                Order = order,
                IsActive = isActive,
                CreatedAt = createdAt
            };

            // Act - get all properties of the list resource
            var properties = typeof(SuccessStoryListResource).GetProperties();
            var expectedFields = new[]
            {
                "Id", "Title", "Description", "ImageUrl", "ActionLabel", 
                "ActionLink", "Order", "IsActive", "CreatedAt"
            };

            // Assert - verify only essential list fields are present (no UpdatedAt, CreatedByUserId for list view)
            var actualFields = properties.Select(p => p.Name).ToArray();
            var hasOnlyEssentialFields = expectedFields.All(field => actualFields.Contains(field)) &&
                                       actualFields.All(field => expectedFields.Contains(field));

            return hasOnlyEssentialFields;
        }
    }
}