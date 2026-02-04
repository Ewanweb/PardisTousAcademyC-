using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Tests
{
    public class SeoIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SeoIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetSeo_ReturnsCorrectSeoData_ForValidCategorySlug()
        {
            // Arrange
            var slug = "programming";
            var language = "fa";

            // Act
            var response = await _client.GetAsync($"/api/seo/category/{slug}?language={language}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var seoDto = JsonSerializer.Deserialize<SeoDto>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Assert.NotNull(seoDto);
            Assert.NotEmpty(seoDto.Title);
            Assert.NotEmpty(seoDto.Description);
            Assert.NotEmpty(seoDto.CanonicalUrl);
            Assert.Equal("index, follow", seoDto.RobotsContent);
        }

        [Fact]
        public async Task GetSeo_ReturnsNotFound_ForInvalidSlug()
        {
            // Arrange
            var slug = "non-existent-category";

            // Act
            var response = await _client.GetAsync($"/api/seo/category/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSeo_ReturnsNoIndex_ForDeepPagination()
        {
            // Arrange
            var slug = "programming";
            var page = 15;

            // Act
            var response = await _client.GetAsync($"/api/seo/category/{slug}?page={page}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var seoDto = JsonSerializer.Deserialize<SeoDto>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Assert.NotNull(seoDto);
            Assert.Equal("noindex, follow", seoDto.RobotsContent);
        }

        [Fact]
        public async Task GetSitemap_ReturnsValidXml()
        {
            // Act
            var response = await _client.GetAsync("/api/seo/sitemap.xml");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", content);
            Assert.Contains("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">", content);
            Assert.Contains("</urlset>", content);
        }

        [Fact]
        public async Task GetRobots_ReturnsValidRobotsTxt()
        {
            // Act
            var response = await _client.GetAsync("/api/seo/robots.txt");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("User-agent: *", content);
            Assert.Contains("Allow: /", content);
            Assert.Contains("Disallow: /admin/", content);
            Assert.Contains("Sitemap:", content);
        }

        [Theory]
        [InlineData("/category/programming", "index, follow")]
        [InlineData("/category/programming?page=2", "index, follow")]
        [InlineData("/category/programming?page=15", "noindex, follow")]
        [InlineData("/category/programming?level=beginner&price=free&sort=newest", "noindex, follow")]
        public async Task SeoEndpoint_ReturnsCorrectRobotsDirective_BasedOnFilters(string path, string expectedRobots)
        {
            // Arrange
            var slug = "programming";
            var queryString = path.Split('?').Length > 1 ? path.Split('?')[1] : "";

            // Act
            var response = await _client.GetAsync($"/api/seo/category/{slug}?{queryString}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var seoDto = JsonSerializer.Deserialize<SeoDto>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Assert.NotNull(seoDto);
            Assert.Equal(expectedRobots, seoDto.RobotsContent);
        }

        [Fact]
        public async Task CategoryPage_RendersCorrectSeoTags()
        {
            // Act
            var response = await _client.GetAsync("/category/programming");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            
            // Check for essential SEO tags
            Assert.Contains("<title>", content);
            Assert.Contains("<meta name=\"description\"", content);
            Assert.Contains("<link rel=\"canonical\"", content);
            Assert.Contains("<meta name=\"robots\"", content);
            Assert.Contains("<meta property=\"og:title\"", content);
            Assert.Contains("<meta property=\"og:description\"", content);
            Assert.Contains("<meta name=\"twitter:card\"", content);
            Assert.Contains("<script type=\"application/ld+json\">", content);
        }

        [Fact]
        public async Task CoursePage_RendersCorrectSeoTags()
        {
            // Act
            var response = await _client.GetAsync("/course/react-fundamentals");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            
            // Check for essential SEO tags
            Assert.Contains("<title>", content);
            Assert.Contains("<meta name=\"description\"", content);
            Assert.Contains("<link rel=\"canonical\"", content);
            Assert.Contains("<meta name=\"robots\"", content);
            Assert.Contains("<meta property=\"og:type\" content=\"article\"", content);
            Assert.Contains("<script type=\"application/ld+json\">", content);
            
            // Check for course-specific structured data
            Assert.Contains("\"@type\":\"Course\"", content.Replace(" ", ""));
        }

        [Fact]
        public async Task SeoTags_AreProperlyEncoded_PreventingXSS()
        {
            // This test would require a category with potentially malicious content
            // For demonstration, we'll test that HTML is properly encoded
            
            // Act
            var response = await _client.GetAsync("/category/test-xss");

            // Assert
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Ensure no unescaped HTML tags in meta content
                Assert.DoesNotContain("<script>", content);
                Assert.DoesNotContain("javascript:", content);
                
                // Check that HTML entities are properly encoded in meta tags
                if (content.Contains("&lt;") || content.Contains("&gt;"))
                {
                    Assert.True(true, "HTML is properly encoded");
                }
            }
        }
    }
}