using System;
using FsCheck;
using FsCheck.Xunit;
using Pardis.Domain.Sliders;
using Xunit;

namespace Pardis.Domain.Tests.Sliders
{
    /// <summary>
    /// Property-based tests for partial update handling
    /// Feature: simplified-slider-system, Property 6: Partial Update Handling
    /// Validates: Requirements 4.4, 3.3
    /// </summary>
    public class PartialUpdateHandlingTests
    {
        [Property(MaxTest = 100)]
        public bool HeroSlide_Entity_Update_Method_Should_Handle_Partial_Updates_Correctly(
            string? newTitle, string? newDescription, string? newImageUrl, 
            string? newActionLabel, string? newActionLink, int? newOrder, bool? newIsActive)
        {
            // Arrange - create hero slide with known values
            var slide = HeroSlide.Create(
                title: "Original Title",
                imageUrl: "https://original.com/image.jpg",
                createdByUserId: Guid.NewGuid(),
                description: "Original Description",
                actionLabel: "Original Action",
                actionLink: "https://original.com",
                order: 5
            );

            // Store original values
            var originalTitle = slide.Title;
            var originalDescription = slide.Description;
            var originalImageUrl = slide.ImageUrl;
            var originalActionLabel = slide.ActionLabel;
            var originalActionLink = slide.ActionLink;
            var originalOrder = slide.Order;
            var originalIsActive = slide.IsActive;

            // Act - perform partial update
            slide.Update(
                title: newTitle,
                description: newDescription,
                imageUrl: newImageUrl,
                actionLabel: newActionLabel,
                actionLink: newActionLink,
                order: newOrder,
                isActive: newIsActive
            );

            // Assert - verify only provided fields were updated
            var titleCorrect = !string.IsNullOrEmpty(newTitle) ? slide.Title == newTitle : slide.Title == originalTitle;
            var descriptionCorrect = newDescription != null ? slide.Description == newDescription : slide.Description == originalDescription;
            var imageUrlCorrect = !string.IsNullOrEmpty(newImageUrl) ? slide.ImageUrl == newImageUrl : slide.ImageUrl == originalImageUrl;
            var actionLabelCorrect = newActionLabel != null ? slide.ActionLabel == newActionLabel : slide.ActionLabel == originalActionLabel;
            var actionLinkCorrect = newActionLink != null ? slide.ActionLink == newActionLink : slide.ActionLink == originalActionLink;
            var orderCorrect = newOrder.HasValue ? slide.Order == newOrder.Value : slide.Order == originalOrder;
            var isActiveCorrect = newIsActive.HasValue ? slide.IsActive == newIsActive.Value : slide.IsActive == originalIsActive;

            return titleCorrect && descriptionCorrect && imageUrlCorrect && 
                   actionLabelCorrect && actionLinkCorrect && orderCorrect && isActiveCorrect;
        }

        [Property(MaxTest = 100)]
        public bool SuccessStory_Entity_Update_Method_Should_Handle_Partial_Updates_Correctly(
            string? newTitle, string? newDescription, string? newImageUrl, 
            string? newActionLabel, string? newActionLink, int? newOrder, bool? newIsActive)
        {
            // Arrange - create success story with known values
            var story = SuccessStory.Create(
                title: "Original Title",
                imageUrl: "https://original.com/image.jpg",
                createdByUserId: Guid.NewGuid(),
                description: "Original Description",
                actionLabel: "Original Action",
                actionLink: "https://original.com",
                order: 5
            );

            // Store original values
            var originalTitle = story.Title;
            var originalDescription = story.Description;
            var originalImageUrl = story.ImageUrl;
            var originalActionLabel = story.ActionLabel;
            var originalActionLink = story.ActionLink;
            var originalOrder = story.Order;
            var originalIsActive = story.IsActive;

            // Act - perform partial update
            story.Update(
                title: newTitle,
                description: newDescription,
                imageUrl: newImageUrl,
                actionLabel: newActionLabel,
                actionLink: newActionLink,
                order: newOrder,
                isActive: newIsActive
            );

            // Assert - verify only provided fields were updated
            var titleCorrect = !string.IsNullOrEmpty(newTitle) ? story.Title == newTitle : story.Title == originalTitle;
            var descriptionCorrect = newDescription != null ? story.Description == newDescription : story.Description == originalDescription;
            var imageUrlCorrect = !string.IsNullOrEmpty(newImageUrl) ? story.ImageUrl == newImageUrl : story.ImageUrl == originalImageUrl;
            var actionLabelCorrect = newActionLabel != null ? story.ActionLabel == newActionLabel : story.ActionLabel == originalActionLabel;
            var actionLinkCorrect = newActionLink != null ? story.ActionLink == newActionLink : story.ActionLink == originalActionLink;
            var orderCorrect = newOrder.HasValue ? story.Order == newOrder.Value : story.Order == originalOrder;
            var isActiveCorrect = newIsActive.HasValue ? story.IsActive == newIsActive.Value : story.IsActive == originalIsActive;

            return titleCorrect && descriptionCorrect && imageUrlCorrect && 
                   actionLabelCorrect && actionLinkCorrect && orderCorrect && isActiveCorrect;
        }
    }
}