using MediatR;
using Microsoft.AspNetCore.Http;
using Pardis.Application._Shared;

namespace Pardis.Application.Blog.Posts.UploadImage;

public record UploadBlogImageCommand(IFormFile ImageFile, string? CurrentUserId) : IRequest<OperationResult<UploadBlogImageResult>>;

public class UploadBlogImageResult
{
    public string ImageUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
}