using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;

namespace Pardis.Application.Blog.Posts.UploadImage;

public class UploadBlogImageCommandHandler : IRequestHandler<UploadBlogImageCommand, OperationResult<UploadBlogImageResult>>
{
    private readonly ISecureFileService _secureFileService;
    private readonly ILogger<UploadBlogImageCommandHandler> _logger;

    public UploadBlogImageCommandHandler(
        ISecureFileService secureFileService,
        ILogger<UploadBlogImageCommandHandler> logger)
    {
        _secureFileService = secureFileService;
        _logger = logger;
    }

    public async Task<OperationResult<UploadBlogImageResult>> Handle(UploadBlogImageCommand request, CancellationToken cancellationToken)
    {
        if (request.ImageFile == null || request.ImageFile.Length == 0)
            return OperationResult<UploadBlogImageResult>.Error("فایل تصویر انتخاب نشده است");

        try
        {
            // Validate image file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return OperationResult<UploadBlogImageResult>.Error("فرمت تصویر مجاز نیست. فقط JPG, PNG, GIF و WebP مجاز است");

            // Validate file size (max 5MB for images)
            const int maxFileSize = 5 * 1024 * 1024;
            if (request.ImageFile.Length > maxFileSize)
                return OperationResult<UploadBlogImageResult>.Error("حجم تصویر نباید بیشتر از 5 مگابایت باشد");

            // Upload using secure file service
            var uploadResult = await _secureFileService.SaveFileSecurely(
                request.ImageFile,
                "blog-images",
                request.CurrentUserId
            );

            if (!uploadResult.IsSuccess)
            {
                _logger.LogWarning("Failed to upload blog image. Error: {Error}", uploadResult.ErrorMessage);
                return OperationResult<UploadBlogImageResult>.Error(uploadResult.ErrorMessage ?? "خطا در آپلود تصویر");
            }

            // Generate public URL for the image
            var imageUrl = $"/uploads/blog-images/{uploadResult.SecureFileName}";

            var result = new UploadBlogImageResult
            {
                ImageUrl = imageUrl,
                AccessToken = uploadResult.AccessToken ?? string.Empty,
                FileName = uploadResult.SecureFileName ?? string.Empty,
                FileSize = uploadResult.FileSize
            };

            _logger.LogInformation("Blog image uploaded successfully. FileName: {FileName}, UserId: {UserId}",
                uploadResult.SecureFileName, request.CurrentUserId);

            return OperationResult<UploadBlogImageResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blog image. UserId: {UserId}", request.CurrentUserId);
            return OperationResult<UploadBlogImageResult>.Error("خطای غیرمنتظره در آپلود تصویر");
        }
    }
}