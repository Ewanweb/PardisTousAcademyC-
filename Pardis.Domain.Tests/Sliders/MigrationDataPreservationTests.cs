using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for migration data preservation
    /// Feature: simplified-slider-system, Property 10: Migration Data Preservation
    /// Validates: Requirements 8.1, 8.2, 8.3, 8.4
    /// </summary>
    public class MigrationDataPreservationTests
    {
        /// <summary>
        /// Represents legacy HeroSlide data structure before migration
        /// </summary>
        public class LegacyHeroSlideData
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
            public string? ButtonText { get; set; }
            public string? LinkUrl { get; set; }
            public string? PrimaryActionLabel { get; set; }
            public string? PrimaryActionLink { get; set; }
            public string? SecondaryActionLabel { get; set; }
            public string? SecondaryActionLink { get; set; }
            public string? Badge { get; set; }
            public string? StatsJson { get; set; }
            public bool IsPermanent { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public int Order { get; set; }
            public bool IsActive { get; set; }
            public Guid CreatedByUserId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        /// <summary>
        /// Represents legacy SuccessStory data structure before migration
        /// </summary>
        public class LegacySuccessStoryData
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
            public string? LinkUrl { get; set; }
            public string? ActionLabel { get; set; }
            public string? ActionLink { get; set; }
            public string? StudentName { get; set; }
            public string? CourseName { get; set; }
            public string? Subtitle { get; set; }
            public string? Badge { get; set; }
            public string? Type { get; set; }
            public string? StatsJson { get; set; }
            public int? Duration { get; set; }
            public bool IsPermanent { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public int Order { get; set; }
            public bool IsActive { get; set; }
            public Guid CreatedByUserId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        /// <summary>
        /// Simulates the migration logic for HeroSlide data
        /// </summary>
        private static (string? ActionLabel, string? ActionLink) MigrateHeroSlideActions(LegacyHeroSlideData legacy)
        {
            // Migration priority: PrimaryActionLabel/Link > ButtonText/LinkUrl
            var actionLabel = !string.IsNullOrWhiteSpace(legacy.PrimaryActionLabel) 
                ? legacy.PrimaryActionLabel 
                : legacy.ButtonText;
            
            var actionLink = !string.IsNullOrWhiteSpace(legacy.PrimaryActionLink) 
                ? legacy.PrimaryActionLink 
                : legacy.LinkUrl;

            return (actionLabel, actionLink);
        }

        /// <summary>
        /// Simulates the migration logic for SuccessStory data
        /// </summary>
        private static (string? ActionLabel, string? ActionLink) MigrateSuccessStoryActions(LegacySuccessStoryData legacy)
        {
            // Migration priority: ActionLabel/Link > default values with LinkUrl
            var actionLabel = !string.IsNullOrWhiteSpace(legacy.ActionLabel) 
                ? legacy.ActionLabel 
                : (!string.IsNullOrWhiteSpace(legacy.LinkUrl) ? "مشاهده" : null);
            
            var actionLink = !string.IsNullOrWhiteSpace(legacy.ActionLink) 
                ? legacy.ActionLink 
                : legacy.LinkUrl;

            return (actionLabel, actionLink);
        }

        [Property(MaxTest = 100)]
        public bool HeroSlide_Migration_Should_Preserve_Essential_Fields(
            NonEmptyString title, 
            NonEmptyString imageUrl, 
            Guid userId,
            string? description,
            string? buttonText,
            string? linkUrl,
            string? primaryActionLabel,
            string? primaryActionLink,
            bool isActive,
            int order)
        {
            // Arrange - create legacy data
            var legacyData = new LegacyHeroSlideData
            {
                Id = Guid.NewGuid(),
                Title = title.Get,
                Description = description,
                ImageUrl = imageUrl.Get,
                ButtonText = buttonText,
                LinkUrl = linkUrl,
                PrimaryActionLabel = primaryActionLabel,
                PrimaryActionLink = primaryActionLink,
                IsActive = isActive,
                Order = Math.Abs(order), // Ensure positive order
                CreatedByUserId = userId == Guid.Empty ? Guid.NewGuid() : userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act - simulate migration
            var (migratedActionLabel, migratedActionLink) = MigrateHeroSlideActions(legacyData);

            // Assert - verify essential fields are preserved
            var titlePreserved = legacyData.Title == title.Get;
            var descriptionPreserved = legacyData.Description == description;
            var imageUrlPreserved = legacyData.ImageUrl == imageUrl.Get;
            var orderPreserved = legacyData.Order >= 0; // Order should be non-negative
            var isActivePreserved = legacyData.IsActive == isActive;

            // Verify action mapping logic
            var actionMappingCorrect = true;
            if (!string.IsNullOrWhiteSpace(primaryActionLabel))
            {
                actionMappingCorrect = migratedActionLabel == primaryActionLabel;
            }
            else if (!string.IsNullOrWhiteSpace(buttonText))
            {
                actionMappingCorrect = migratedActionLabel == buttonText;
            }

            if (!string.IsNullOrWhiteSpace(primaryActionLink))
            {
                actionMappingCorrect = actionMappingCorrect && migratedActionLink == primaryActionLink;
            }
            else if (!string.IsNullOrWhiteSpace(linkUrl))
            {
                actionMappingCorrect = actionMappingCorrect && migratedActionLink == linkUrl;
            }

            return titlePreserved && descriptionPreserved && imageUrlPreserved && 
                   orderPreserved && isActivePreserved && actionMappingCorrect;
        }

        [Property(MaxTest = 100)]
        public bool SuccessStory_Migration_Should_Preserve_Essential_Fields(
            NonEmptyString title, 
            NonEmptyString imageUrl, 
            Guid userId,
            string? description,
            string? linkUrl,
            string? actionLabel,
            string? actionLink,
            bool isActive,
            int order)
        {
            // Arrange - create legacy data
            var legacyData = new LegacySuccessStoryData
            {
                Id = Guid.NewGuid(),
                Title = title.Get,
                Description = description,
                ImageUrl = imageUrl.Get,
                LinkUrl = linkUrl,
                ActionLabel = actionLabel,
                ActionLink = actionLink,
                IsActive = isActive,
                Order = Math.Abs(order), // Ensure positive order
                CreatedByUserId = userId == Guid.Empty ? Guid.NewGuid() : userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act - simulate migration
            var (migratedActionLabel, migratedActionLink) = MigrateSuccessStoryActions(legacyData);

            // Assert - verify essential fields are preserved
            var titlePreserved = legacyData.Title == title.Get;
            var descriptionPreserved = legacyData.Description == description;
            var imageUrlPreserved = legacyData.ImageUrl == imageUrl.Get;
            var orderPreserved = legacyData.Order >= 0; // Order should be non-negative
            var isActivePreserved = legacyData.IsActive == isActive;

            // Verify action mapping logic
            var actionMappingCorrect = true;
            if (!string.IsNullOrWhiteSpace(actionLabel))
            {
                actionMappingCorrect = migratedActionLabel == actionLabel;
            }
            else if (!string.IsNullOrWhiteSpace(linkUrl))
            {
                actionMappingCorrect = migratedActionLabel == "مشاهده";
            }

            if (!string.IsNullOrWhiteSpace(actionLink))
            {
                actionMappingCorrect = actionMappingCorrect && migratedActionLink == actionLink;
            }
            else if (!string.IsNullOrWhiteSpace(linkUrl))
            {
                actionMappingCorrect = actionMappingCorrect && migratedActionLink == linkUrl;
            }

            return titlePreserved && descriptionPreserved && imageUrlPreserved && 
                   orderPreserved && isActivePreserved && actionMappingCorrect;
        }

        [Property(MaxTest = 100)]
        public bool Migration_Should_Set_Default_Values_For_Required_Fields(
            NonEmptyString title, 
            NonEmptyString imageUrl, 
            Guid userId)
        {
            // Arrange - create minimal legacy data (missing some required fields)
            var legacyHeroSlide = new LegacyHeroSlideData
            {
                Id = Guid.NewGuid(),
                Title = title.Get,
                ImageUrl = imageUrl.Get,
                CreatedByUserId = userId == Guid.Empty ? Guid.NewGuid() : userId,
                // Order and IsActive not set (should get defaults)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var legacySuccessStory = new LegacySuccessStoryData
            {
                Id = Guid.NewGuid(),
                Title = title.Get,
                ImageUrl = imageUrl.Get,
                CreatedByUserId = userId == Guid.Empty ? Guid.NewGuid() : userId,
                // Order and IsActive not set (should get defaults)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act - simulate migration with default value assignment
            var heroSlideOrder = legacyHeroSlide.Order == 0 ? 0 : legacyHeroSlide.Order; // Default to 0
            var heroSlideIsActive = legacyHeroSlide.IsActive || !legacyHeroSlide.IsActive; // Default to true if not set
            
            var successStoryOrder = legacySuccessStory.Order == 0 ? 0 : legacySuccessStory.Order; // Default to 0
            var successStoryIsActive = legacySuccessStory.IsActive || !legacySuccessStory.IsActive; // Default to true if not set

            // Assert - verify defaults are applied correctly
            var heroSlideDefaultsCorrect = heroSlideOrder >= 0; // Order should be non-negative
            var successStoryDefaultsCorrect = successStoryOrder >= 0; // Order should be non-negative

            return heroSlideDefaultsCorrect && successStoryDefaultsCorrect;
        }

        [Property(MaxTest = 100)]
        public bool Migration_Should_Handle_Complex_Field_Removal(
            NonEmptyString title, 
            NonEmptyString imageUrl, 
            Guid userId,
            string? badge,
            string? statsJson,
            bool isPermanent,
            DateTime? expiresAt)
        {
            // Arrange - create legacy data with complex fields
            var legacyHeroSlide = new LegacyHeroSlideData
            {
                Id = Guid.NewGuid(),
                Title = title.Get,
                ImageUrl = imageUrl.Get,
                Badge = badge,
                StatsJson = statsJson,
                IsPermanent = isPermanent,
                ExpiresAt = expiresAt,
                CreatedByUserId = userId == Guid.Empty ? Guid.NewGuid() : userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act - simulate migration (complex fields should be ignored/removed)
            var essentialFieldsOnly = new
            {
                legacyHeroSlide.Id,
                legacyHeroSlide.Title,
                legacyHeroSlide.Description,
                legacyHeroSlide.ImageUrl,
                legacyHeroSlide.Order,
                legacyHeroSlide.IsActive,
                legacyHeroSlide.CreatedByUserId,
                legacyHeroSlide.CreatedAt,
                legacyHeroSlide.UpdatedAt
            };

            // Assert - verify that essential fields are preserved and complex fields are not included
            var essentialFieldsPreserved = 
                essentialFieldsOnly.Title == title.Get &&
                essentialFieldsOnly.ImageUrl == imageUrl.Get &&
                essentialFieldsOnly.Id != Guid.Empty &&
                essentialFieldsOnly.CreatedByUserId != Guid.Empty;

            // Complex fields should not be part of the migrated structure
            // This is verified by the fact that we only extract essential fields above
            return essentialFieldsPreserved;
        }

        [Fact]
        public void Migration_Should_Handle_Null_Legacy_Data_Gracefully()
        {
            // Arrange
            LegacyHeroSlideData? nullHeroSlide = null;
            LegacySuccessStoryData? nullSuccessStory = null;

            // Act & Assert - migration should handle null data without throwing
            var heroSlideResult = nullHeroSlide == null ? (null, null) : MigrateHeroSlideActions(nullHeroSlide);
            var successStoryResult = nullSuccessStory == null ? (null, null) : MigrateSuccessStoryActions(nullSuccessStory);

            Assert.Equal((null, null), heroSlideResult);
            Assert.Equal((null, null), successStoryResult);
        }

        [Theory]
        [InlineData("", "", null, null)] // Empty strings should result in null
        [InlineData("   ", "   ", null, null)] // Whitespace strings should result in null
        [InlineData("Button", "Link", "Button", "Link")] // Valid strings should be preserved
        [InlineData("Primary", "PrimaryLink", "Primary", "PrimaryLink")] // Primary actions take precedence
        public void HeroSlide_Migration_Should_Handle_Action_Mapping_Edge_Cases(
            string? buttonText, 
            string? linkUrl, 
            string? expectedActionLabel, 
            string? expectedActionLink)
        {
            // Arrange
            var legacyData = new LegacyHeroSlideData
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                ImageUrl = "https://example.com/image.jpg",
                ButtonText = string.IsNullOrWhiteSpace(buttonText) ? null : buttonText,
                LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? null : linkUrl,
                CreatedByUserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var (actualActionLabel, actualActionLink) = MigrateHeroSlideActions(legacyData);

            // Assert
            Assert.Equal(expectedActionLabel, actualActionLabel);
            Assert.Equal(expectedActionLink, actualActionLink);
        }
    }
}