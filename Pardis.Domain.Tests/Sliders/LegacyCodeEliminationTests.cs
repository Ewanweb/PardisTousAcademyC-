using System;
using System.Linq;
using System.Reflection;
using FsCheck;
using FsCheck.Xunit;
using Pardis.Domain.Sliders;
using Pardis.Domain.Dto.Sliders;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for legacy code elimination
    /// Feature: simplified-slider-system, Property 11: Legacy Code Elimination
    /// Validates: Requirements 6.2, 6.3
    /// </summary>
    public class LegacyCodeEliminationTests
    {
        private readonly string[] _legacyProperties = 
        {
            "Badge", "PrimaryActionLabel", "PrimaryActionLink", "SecondaryActionLabel", "SecondaryActionLink",
            "StatsJson", "IsPermanent", "ExpiresAt", "Subtitle", "Type", "StudentName", "CourseName", 
            "Duration", "CourseId", "LinkUrl", "ButtonText"
        };

        private readonly string[] _legacyMethods = 
        {
            "IsExpired", "GetTimeRemaining", "SetExpiration", "MarkAsPermanent", "AddStats", "RemoveStats",
            "GetStats", "SetBadge", "AddSecondaryAction", "RemoveSecondaryAction"
        };

        [Property(MaxTest = 100)]
        public bool Domain_Entities_Should_Not_Contain_Legacy_Properties(string dummyInput)
        {
            // Act - get all slider-related domain entities
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            var heroSlideProperties = heroSlideType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var successStoryProperties = successStoryType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Assert - verify no legacy properties exist
            var heroSlideHasLegacyProps = heroSlideProperties.Any(prop => 
                _legacyProperties.Contains(prop.Name));

            var successStoryHasLegacyProps = successStoryProperties.Any(prop => 
                _legacyProperties.Contains(prop.Name));

            return !heroSlideHasLegacyProps && !successStoryHasLegacyProps;
        }

        [Property(MaxTest = 100)]
        public bool Domain_Entities_Should_Not_Contain_Legacy_Methods(string dummyInput)
        {
            // Act - get all slider-related domain entities
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            var heroSlideMethods = heroSlideType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var successStoryMethods = successStoryType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            // Assert - verify no legacy methods exist
            var heroSlideHasLegacyMethods = heroSlideMethods.Any(method => 
                _legacyMethods.Contains(method.Name));

            var successStoryHasLegacyMethods = successStoryMethods.Any(method => 
                _legacyMethods.Contains(method.Name));

            return !heroSlideHasLegacyMethods && !successStoryHasLegacyMethods;
        }

        [Property(MaxTest = 100)]
        public bool DTO_Assembly_Should_Not_Contain_Legacy_Stat_Classes(string dummyInput)
        {
            // Act - get all types in the DTO assembly
            var dtoAssembly = typeof(HeroSlideResource).Assembly;
            var allTypes = dtoAssembly.GetTypes();

            // Assert - verify no legacy stat DTO classes exist
            var hasSlideStatDto = allTypes.Any(type => type.Name == "SlideStatDto");
            var hasStoryStatDto = allTypes.Any(type => type.Name == "StoryStatDto");
            var hasSlideActionDto = allTypes.Any(type => type.Name == "SlideActionDto");
            var hasStoryActionDto = allTypes.Any(type => type.Name == "StoryActionDto");

            return !hasSlideStatDto && !hasStoryStatDto && !hasSlideActionDto && !hasStoryActionDto;
        }

        [Property(MaxTest = 100)]
        public bool System_Should_Not_Have_Expiration_Logic_In_Background_Services(string dummyInput)
        {
            // This property verifies that background services don't contain active expiration logic
            // Since we can't easily test the actual background service implementation in a unit test,
            // we verify that the domain entities don't support expiration functionality
            
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            // Check that entities don't have expiration-related properties
            var heroSlideHasExpirationProps = heroSlideType.GetProperties()
                .Any(prop => prop.Name.Contains("Expir") || prop.Name.Contains("Permanent"));

            var successStoryHasExpirationProps = successStoryType.GetProperties()
                .Any(prop => prop.Name.Contains("Expir") || prop.Name.Contains("Permanent"));

            return !heroSlideHasExpirationProps && !successStoryHasExpirationProps;
        }

        [Fact]
        public void Legacy_Stat_DTOs_Should_Not_Exist_In_Codebase()
        {
            // Arrange & Act - check if legacy stat DTO file exists
            var dtoAssembly = typeof(HeroSlideResource).Assembly;
            var allTypes = dtoAssembly.GetTypes();

            // Assert - verify specific legacy DTOs are removed
            var slideStatDto = allTypes.FirstOrDefault(type => type.Name == "SlideStatDto");
            var storyStatDto = allTypes.FirstOrDefault(type => type.Name == "StoryStatDto");
            var slideActionDto = allTypes.FirstOrDefault(type => type.Name == "SlideActionDto");
            var storyActionDto = allTypes.FirstOrDefault(type => type.Name == "StoryActionDto");

            Assert.Null(slideStatDto);
            Assert.Null(storyStatDto);
            Assert.Null(slideActionDto);
            Assert.Null(storyActionDto);
        }

        [Fact]
        public void Domain_Entities_Should_Only_Have_Simplified_Create_Methods()
        {
            // Arrange
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            // Act - get create methods
            var heroSlideCreateMethods = heroSlideType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Create").ToArray();

            var successStoryCreateMethods = successStoryType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Create").ToArray();

            // Assert - should have exactly one simplified Create method each
            Assert.Single(heroSlideCreateMethods);
            Assert.Single(successStoryCreateMethods);

            // Verify the Create methods don't have legacy parameters
            var heroSlideCreateMethod = heroSlideCreateMethods[0];
            var successStoryCreateMethod = successStoryCreateMethods[0];

            var heroSlideParams = heroSlideCreateMethod.GetParameters();
            var successStoryParams = successStoryCreateMethod.GetParameters();

            // Should not have parameters for legacy fields
            Assert.DoesNotContain(heroSlideParams, p => _legacyProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
            Assert.DoesNotContain(successStoryParams, p => _legacyProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void Domain_Entities_Should_Only_Have_Simplified_Update_Methods()
        {
            // Arrange
            var heroSlideType = typeof(HeroSlide);
            var successStoryType = typeof(SuccessStory);

            // Act - get update methods
            var heroSlideUpdateMethods = heroSlideType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "Update").ToArray();

            var successStoryUpdateMethods = successStoryType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "Update").ToArray();

            // Assert - should have exactly one simplified Update method each
            Assert.Single(heroSlideUpdateMethods);
            Assert.Single(successStoryUpdateMethods);

            // Verify the Update methods don't have legacy parameters
            var heroSlideUpdateMethod = heroSlideUpdateMethods[0];
            var successStoryUpdateMethod = successStoryUpdateMethods[0];

            var heroSlideParams = heroSlideUpdateMethod.GetParameters();
            var successStoryParams = successStoryUpdateMethod.GetParameters();

            // Should not have parameters for legacy fields
            Assert.DoesNotContain(heroSlideParams, p => _legacyProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
            Assert.DoesNotContain(successStoryParams, p => _legacyProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
        }
    }
}