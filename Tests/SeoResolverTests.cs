using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pardis.Application.Seo;
using Pardis.Application.Seo.Resolvers;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;
using Xunit;

namespace Tests
{
    public class SeoResolverTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly CategorySeoResolver _categoryResolver;
        private readonly CourseSeoResolver _courseResolver;

        public SeoResolverTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _categoryResolver = new CategorySeoResolver(_categoryRepositoryMock.Object);
            _courseResolver = new CourseSeoResolver(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task CategoryResolver_GeneratesDefaultSeo_WhenSeoDataIsNull()
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa"
            };

            // Act
            var result = await _categoryResolver.ResolveAsync(category, context);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Test Category", result.Title);
            Assert.Contains("آکادمی پردیس توس", result.Title);
            Assert.NotEmpty(result.Description);
            Assert.Equal("https://test.com/category/test-category", result.CanonicalUrl);
            Assert.Equal("index, follow", result.RobotsContent);
        }

        [Fact]
        public async Task CategoryResolver_HandlesDeepPagination_WithNoIndex()
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa",
                Page = 15 // Deep pagination
            };

            // Act
            var result = await _categoryResolver.ResolveAsync(category, context);

            // Assert
            Assert.Equal("noindex, follow", result.RobotsContent);
            Assert.Contains("صفحه 15", result.Title);
        }

        [Fact]
        public async Task CategoryResolver_HandlesComplexFilters_WithNoIndex()
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa",
                QueryParams = new Dictionary<string, string>
                {
                    ["level"] = "beginner",
                    ["price"] = "free",
                    ["sort"] = "newest"
                }
            };

            // Act
            var result = await _categoryResolver.ResolveAsync(category, context);

            // Assert
            Assert.Equal("noindex, follow", result.RobotsContent);
        }

        [Fact]
        public async Task CourseResolver_GeneratesCorrectJsonLd_ForPublishedCourse()
        {
            // Arrange
            var instructor = new User("instructor@test.com", "Test Instructor");
            var category = new Category("programming", "Programming", "Programming courses");
            var course = new Course(
                "test-course",
                "Test Course",
                "Test course description",
                category.Id,
                instructor.Id,
                1000,
                CourseStatus.Published
            );
            course.SetDuration(40);
            course.SetLevel("intermediate");

            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa"
            };

            // Act
            var result = await _courseResolver.ResolveAsync(course, context);

            // Assert
            Assert.NotEmpty(result.JsonLdSchemas);
            var courseSchema = result.JsonLdSchemas.FirstOrDefault();
            Assert.NotNull(courseSchema);
            
            // Verify schema contains required fields
            var schemaDict = courseSchema as Dictionary<string, object>;
            Assert.NotNull(schemaDict);
            Assert.Equal("Course", schemaDict["type"]);
            Assert.Equal("Test Course", schemaDict["name"]);
        }

        [Fact]
        public async Task CourseResolver_ReturnsNoIndex_ForUnpublishedCourse()
        {
            // Arrange
            var course = new Course(
                "test-course",
                "Test Course",
                "Test course description",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000,
                CourseStatus.Draft
            );

            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa"
            };

            // Act
            var result = await _courseResolver.ResolveAsync(course, context);

            // Assert
            Assert.Equal("noindex, nofollow", result.RobotsContent);
        }

        [Fact]
        public void CategoryResolver_BuildsCorrectCanonicalUrl_WithPagination()
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa",
                Page = 2,
                QueryParams = new Dictionary<string, string>
                {
                    ["page"] = "2",
                    ["level"] = "beginner"
                }
            };

            // Act
            var canonicalUrl = _categoryResolver.BuildCanonicalUrl(category, context);

            // Assert
            Assert.Equal("https://test.com/category/test-category?level=beginner&page=2", canonicalUrl);
        }

        [Fact]
        public void CategoryResolver_BuildsCorrectCanonicalUrl_WithoutPageParameter_ForPageOne()
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa",
                Page = 1,
                QueryParams = new Dictionary<string, string>
                {
                    ["page"] = "1",
                    ["level"] = "beginner"
                }
            };

            // Act
            var canonicalUrl = _categoryResolver.BuildCanonicalUrl(category, context);

            // Assert
            Assert.Equal("https://test.com/category/test-category?level=beginner", canonicalUrl);
        }

        [Fact]
        public void CategoryResolver_GeneratesBreadcrumbs_WithParentCategory()
        {
            // Arrange
            var parentCategory = new Category("programming", "Programming", "Programming courses");
            var childCategory = new Category("web-development", "Web Development", "Web development courses");
            childCategory.SetParent(parentCategory);

            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = "fa"
            };

            // Act
            var breadcrumbs = _categoryResolver.GenerateBreadcrumbs(childCategory, context);

            // Assert
            Assert.Equal(3, breadcrumbs.Count);
            Assert.Equal("خانه", breadcrumbs[0].Name);
            Assert.Equal("/", breadcrumbs[0].Url);
            Assert.Equal("Programming", breadcrumbs[1].Name);
            Assert.Equal("/category/programming", breadcrumbs[1].Url);
            Assert.Equal("Web Development", breadcrumbs[2].Name);
            Assert.Equal("/category/web-development", breadcrumbs[2].Url);
        }

        [Theory]
        [InlineData("fa", "rtl", "fa_IR")]
        [InlineData("en", "ltr", "en_US")]
        public async Task Resolvers_HandleMultipleLanguages_Correctly(string language, string expectedDirection, string expectedLocale)
        {
            // Arrange
            var category = new Category("test-category", "Test Category", "Test description");
            var context = new SeoContext
            {
                BaseUrl = "https://test.com",
                Language = language
            };

            // Act
            var result = await _categoryResolver.ResolveAsync(category, context);

            // Assert
            Assert.Equal(language, result.Language);
            Assert.Equal(expectedDirection, result.Direction);
            Assert.Equal(expectedLocale, result.OgLocale);
        }
    }
}