using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Dto.Seo;
using Xunit;

namespace Pardis.Tests.Integration;

public class BlogIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BlogIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPosts_ShouldReturnPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/api/blog/posts?page=1&pageSize=12");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Items);
        Assert.True(result.Data.Page >= 1);
        Assert.True(result.Data.PageSize >= 1);
        Assert.True(result.Data.TotalCount >= 0);
        Assert.True(result.Data.TotalPages >= 0);
    }

    [Fact]
    public async Task GetPostBySlug_WithValidSlug_ShouldReturnPost()
    {
        // Arrange - First get a post to test with
        var postsResponse = await _client.GetAsync("/api/blog/posts?pageSize=1");
        postsResponse.EnsureSuccessStatusCode();
        var postsContent = await postsResponse.Content.ReadAsStringAsync();
        var postsResult = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(postsContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (postsResult?.Data?.Items?.Any() != true)
        {
            // Skip test if no posts exist
            return;
        }

        var testSlug = postsResult.Data.Items.First().Slug;

        // Act
        var response = await _client.GetAsync($"/api/blog/posts/{testSlug}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PostSlugResolveDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        
        if (result.Data.Post != null)
        {
            Assert.Equal(testSlug, result.Data.Post.Slug);
            Assert.NotNull(result.Data.Post.Title);
            Assert.NotNull(result.Data.Post.Content);
            Assert.NotNull(result.Data.Post.Category);
            Assert.NotNull(result.Data.Post.Tags);
            Assert.NotNull(result.Data.Post.Seo);
        }
    }

    [Fact]
    public async Task GetPostBySlug_WithInvalidSlug_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/blog/posts/non-existent-slug-12345");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PostSlugResolveDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success); // Backend returns success with null post for not found
        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Post);
        Assert.False(result.Data.IsRedirect);
    }

    [Fact]
    public async Task GetCategories_ShouldReturnCategoriesList()
    {
        // Act
        var response = await _client.GetAsync("/api/blog/categories");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<List<BlogCategoryDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        
        foreach (var category in result.Data)
        {
            Assert.NotNull(category.Id);
            Assert.NotNull(category.Title);
            Assert.NotNull(category.Slug);
            Assert.NotNull(category.Seo);
        }
    }

    [Fact]
    public async Task GetTags_ShouldReturnTagsList()
    {
        // Act
        var response = await _client.GetAsync("/api/blog/tags");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<List<TagDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        
        foreach (var tag in result.Data)
        {
            Assert.NotNull(tag.Id);
            Assert.NotNull(tag.Title);
            Assert.NotNull(tag.Slug);
        }
    }

    [Fact]
    public async Task SearchPosts_ShouldReturnHighlightedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/blog/search?q=test&page=1&pageSize=12");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Items);
        
        // Check if any results have highlighting (if search term matches)
        foreach (var post in result.Data.Items)
        {
            Assert.NotNull(post.Title);
            Assert.NotNull(post.Slug);
            Assert.NotNull(post.Excerpt);
            // HighlightedTitle and HighlightedExcerpt may be null if no matches
        }
    }

    [Fact]
    public async Task IncrementView_ShouldSucceed()
    {
        // Arrange - Get a post slug first
        var postsResponse = await _client.GetAsync("/api/blog/posts?pageSize=1");
        postsResponse.EnsureSuccessStatusCode();
        var postsContent = await postsResponse.Content.ReadAsStringAsync();
        var postsResult = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(postsContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (postsResult?.Data?.Items?.Any() != true)
        {
            return; // Skip if no posts
        }

        var testSlug = postsResult.Data.Items.First().Slug;

        // Act
        var response = await _client.PostAsync($"/api/blog/posts/{testSlug}/view", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetPostsByCategory_ShouldReturnFilteredResults()
    {
        // Arrange - Get a category first
        var categoriesResponse = await _client.GetAsync("/api/blog/categories");
        categoriesResponse.EnsureSuccessStatusCode();
        var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
        var categoriesResult = JsonSerializer.Deserialize<ApiResponse<List<BlogCategoryDto>>>(categoriesContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (categoriesResult?.Data?.Any() != true)
        {
            return; // Skip if no categories
        }

        var testCategorySlug = categoriesResult.Data.First().Slug;

        // Act
        var response = await _client.GetAsync($"/api/blog/category/{testCategorySlug}/posts?page=1&pageSize=12");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Items);
        
        // All posts should belong to the specified category
        foreach (var post in result.Data.Items)
        {
            Assert.Equal(testCategorySlug, post.CategorySlug);
        }
    }

    [Fact]
    public async Task GetRelatedPosts_ShouldReturnRelatedPosts()
    {
        // Arrange - Get a post slug first
        var postsResponse = await _client.GetAsync("/api/blog/posts?pageSize=1");
        postsResponse.EnsureSuccessStatusCode();
        var postsContent = await postsResponse.Content.ReadAsStringAsync();
        var postsResult = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(postsContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (postsResult?.Data?.Items?.Any() != true)
        {
            return; // Skip if no posts
        }

        var testSlug = postsResult.Data.Items.First().Slug;

        // Act
        var response = await _client.GetAsync($"/api/blog/posts/{testSlug}/related?take=6");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<List<PostListItemDto>>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Count <= 6); // Should not exceed requested limit
        
        // Related posts should not include the original post
        foreach (var post in result.Data)
        {
            Assert.NotEqual(testSlug, post.Slug);
        }
    }

    [Fact]
    public async Task GetPostNavigation_ShouldReturnNavigation()
    {
        // Arrange - Get a post slug first
        var postsResponse = await _client.GetAsync("/api/blog/posts?pageSize=1");
        postsResponse.EnsureSuccessStatusCode();
        var postsContent = await postsResponse.Content.ReadAsStringAsync();
        var postsResult = JsonSerializer.Deserialize<ApiResponse<PagedResult<PostListItemDto>>>(postsContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (postsResult?.Data?.Items?.Any() != true)
        {
            return; // Skip if no posts
        }

        var testSlug = postsResult.Data.Items.First().Slug;

        // Act
        var response = await _client.GetAsync($"/api/blog/posts/{testSlug}/nav");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PostNavDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        // Next and Previous may be null if no adjacent posts exist
    }
}

// Helper classes for deserialization
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PagedResult<TItem>
{
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
    public object? Stats { get; set; }
}