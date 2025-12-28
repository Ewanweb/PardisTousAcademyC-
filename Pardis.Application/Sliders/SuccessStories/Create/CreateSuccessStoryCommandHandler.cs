using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders._Shared;
using Pardis.Domain.Sliders;

namespace Pardis.Application.Sliders.SuccessStories.Create
{
    public class CreateSuccessStoryCommandHandler : IRequestHandler<CreateSuccessStoryCommand, OperationResult>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly ISliderImageService _imageService;
        private readonly ILogger<CreateSuccessStoryCommandHandler> _logger;

        public CreateSuccessStoryCommandHandler(
            ISuccessStoryRepository successStoryRepository, 
            ISliderImageService imageService,
            ILogger<CreateSuccessStoryCommandHandler> logger)
        {
            _successStoryRepository = successStoryRepository;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (request?.Dto == null)
                {
                    _logger.LogError("CreateSuccessStoryCommand یا Dto آن null است");
                    return OperationResult.Error("درخواست نامعتبر: داده‌های ورودی موجود نیست");
                }

                if (string.IsNullOrWhiteSpace(request.Dto.Title))
                {
                    _logger.LogError("عنوان استوری موفقیت خالی یا null است");
                    return OperationResult.Error("عنوان استوری موفقیت الزامی است");
                }

                // Handle image upload
                string imageUrl = "";
                if (request.Dto.ImageFile != null)
                {
                    try
                    {
                        imageUrl = await _imageService.HandleImageUploadAsync(request.Dto.ImageFile);
                        _logger.LogInformation("فایل تصویر با موفقیت آپلود شد: {ImageUrl}", imageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در آپلود فایل تصویر");
                        return OperationResult.Error($"خطا در آپلود تصویر: {ex.Message}");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(request.Dto.ImageUrl))
                {
                    imageUrl = request.Dto.ImageUrl;
                }
                else
                {
                    _logger.LogError("تصویر ارسال نشده است");
                    return OperationResult.Error("تصویر الزامی است (فایل یا URL)");
                }

                // Validate and parse user ID
                if (string.IsNullOrWhiteSpace(request.CurrentUserId))
                {
                    _logger.LogError("شناسه کاربر موجود نیست");
                    return OperationResult.Error("شناسه کاربر الزامی است");
                }

                if (!Guid.TryParse(request.CurrentUserId, out var userId))
                {
                    _logger.LogError("شناسه کاربر {UserId} نامعتبر است", request.CurrentUserId);
                    return OperationResult.Error("شناسه کاربر نامعتبر است");
                }

                // Create success story with simplified structure
                Domain.Sliders.SuccessStory successStory;
                try
                {
                    successStory = Domain.Sliders.SuccessStory.Create(
                        title: request.Dto.Title,
                        imageUrl: imageUrl,
                        createdByUserId: userId,
                        description: request.Dto.Description,
                        actionLabel: request.Dto.ActionLabel,
                        actionLink: request.Dto.ActionLink,
                        order: request.Dto.Order
                    );
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "خطا در اعتبارسنجی داده‌های استوری موفقیت: {Message}", ex.Message);
                    return OperationResult.Error($"داده‌های ورودی نامعتبر: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ایجاد شیء استوری موفقیت");
                    return OperationResult.Error("خطا در ایجاد استوری موفقیت: داده‌های ورودی نامعتبر");
                }

                // Save to database
                try
                {
                    await _successStoryRepository.AddAsync(successStory);
                    await _successStoryRepository.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogInformation("استوری موفقیت جدید {Id} با موفقیت ایجاد شد. عنوان: {Title}", successStory.Id, successStory.Title);
                    return OperationResult.Success("استوری موفقیت با موفقیت ایجاد شد");
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _logger.LogError(ex, "خطا در ذخیره استوری موفقیت در پایگاه داده. Inner Exception: {InnerException}", ex.InnerException?.Message);
                    return OperationResult.Error("خطا در ذخیره استوری موفقیت: مشکل در پایگاه داده");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطای غیرمنتظره در ذخیره استوری موفقیت");
                    return OperationResult.Error("خطا در ذخیره استوری موفقیت");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("عملیات ایجاد استوری موفقیت لغو شد");
                return OperationResult.Error("عملیات لغو شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره در ایجاد استوری موفقیت جدید. Request: {@Request}", new { 
                    Title = request?.Dto?.Title, 
                    UserId = request?.CurrentUserId,
                    HasImageFile = request?.Dto?.ImageFile != null
                });
                return OperationResult.Error("خطای غیرمنتظره در ایجاد استوری موفقیت");
            }
        }
    }
}