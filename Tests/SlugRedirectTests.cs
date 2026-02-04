using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Application.Seo;
using Pardis.Domain.Seo;
using System.Net;
using Xunit;

namespace Tests
{
    public class SlugRedirectTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SlugRedirectTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Important: Don't follow redirects automatically
            });
        }

        [Fact]
        public async Task SlugRedirect_Returns301_ForOldCategorySlug()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();
            
            await redirectService.CreateRedirectAsync("old-programming", "programming", SeoEntityType.Category);

            // Act
            var response = await _client.GetAsync("/category/old-programming");

            // Assert
            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/category/programming", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task SlugRedirect_Returns301_ForOldCourseSlug()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();
            
            await redirectService.CreateRedirectAsync("old-react-course", "react-fundamentals", SeoEntityType.Course);

            // Act
            var response = await _client.GetAsync("/course/old-react-course");

            // Assert
            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/course/react-fundamentals", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task SlugRedirect_PreservesQueryString_InRedirect()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();
            
            await redirectService.CreateRedirectAsync("old-programming", "programming", SeoEntityType.Category);

            // Act
            var response = await _client.GetAsync("/category/old-programming?page=2&level=beginner");

            // Assert
            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/category/programming?page=2&level=beginner", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task SlugRedirect_DoesNotRedirect_ForNonExistentSlug()
        {
            // Act
            var response = await _client.GetAsync("/category/non-existent-slug");

            // Assert
            Assert.NotEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
            // Should return 404 or the normal response for non-existent category
        }

        [Fact]
        public async Task SlugRedirectService_CreatesRedirect_Successfully()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();

            // Act
            await redirectService.CreateRedirectAsync("test-old-slug", "test-new-slug", SeoEntityType.Category);

            // Assert
            var redirectUrl = await redirectService.GetRedirectUrlAsync("test-old-slug", SeoEntityType.Category);
            Assert.Equal("/category/test-new-slug", redirectUrl);
        }

        [Fact]
        public async Task SlugRedirectService_UpdatesExistingRedirect_WhenCreatingNew()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();

            // Create initial redirect
            await redirectService.CreateRedirectAsync("test-slug", "first-target", SeoEntityType.Category);

            // Act - Create new redirect for same old slug
            await redirectService.CreateRedirectAsync("test-slug", "second-target", SeoEntityType.Category);

            // Assert
            var redirectUrl = await redirectService.GetRedirectUrlAsync("test-slug", SeoEntityType.Category);
            Assert.Equal("/category/second-target", redirectUrl);
        }

        [Fact]
        public async Task SlugRedirectService_RemovesRedirect_Successfully()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();

            await redirectService.CreateRedirectAsync("test-remove-slug", "target-slug", SeoEntityType.Category);

            // Act
            await redirectService.RemoveRedirectAsync("test-remove-slug", SeoEntityType.Category);

            // Assert
            var redirectUrl = await redirectService.GetRedirectUrlAsync("test-remove-slug", SeoEntityType.Category);
            Assert.Null(redirectUrl);
        }

        [Fact]
        public async Task SlugRedirectService_HandlesMultipleEntityTypes_Correctly()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();

            // Create redirects for different entity types with same slug
            await redirectService.CreateRedirectAsync("same-slug", "category-target", SeoEntityType.Category);
            await redirectService.CreateRedirectAsync("same-slug", "course-target", SeoEntityType.Course);

            // Act & Assert
            var categoryRedirect = await redirectService.GetRedirectUrlAsync("same-slug", SeoEntityType.Category);
            var courseRedirect = await redirectService.GetRedirectUrlAsync("same-slug", SeoEntityType.Course);

            Assert.Equal("/category/category-target", categoryRedirect);
            Assert.Equal("/course/course-target", courseRedirect);
        }

        [Theory]
        [InlineData("/category/old-slug")]
        [InlineData("/course/old-slug")]
        [InlineData("/page/old-slug")]
        public async Task SlugRedirectMiddleware_ChecksCorrectPaths_Only(string path)
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();
            
            var entityType = path.StartsWith("/category/") ? SeoEntityType.Category :
                           path.StartsWith("/course/") ? SeoEntityType.Course :
                           SeoEntityType.Page;
            
            await redirectService.CreateRedirectAsync("old-slug", "new-slug", entityType);

            // Act
            var response = await _client.GetAsync(path);

            // Assert
            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        }

        [Fact]
        public async Task SlugRedirectMiddleware_IgnoresNonSeoRoutes()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var redirectService = scope.ServiceProvider.GetRequiredService<ISlugRedirectService>();
            
            await redirectService.CreateRedirectAsync("api", "new-api", SeoEntityType.Category);

            // Act
            var response = await _client.GetAsync("/api/seo/sitemap.xml");

            // Assert
            Assert.NotEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
            // Should process normally, not redirect
        }
    }
}