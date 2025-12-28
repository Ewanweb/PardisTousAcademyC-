using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders._Shared;
using Pardis.Domain.Sliders;

namespace Pardis.Application.Sliders.SuccessStories.Update
{
    public class UpdateSuccessStoryCommandHandler : IRequestHandler<UpdateSuccessStoryCommand, OperationResult>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly ISliderImageService _imageService;
        private readonly ILogger<UpdateSuccessStoryCommandHandler> _logger;

        public UpdateSuccessStoryCommandHandler(
            ISuccessStoryRepository successStoryRepository, 
            ISliderImageService imageService,
            ILogger<UpdateSuccessStoryCommandHandler> logger)
        {
            _successStoryRepository = successStoryRepository;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (request?.Dto == null)
                {
                    _logger.LogError("UpdateSuccessStoryCommand یا Dto آن null است");
                    return OperationResult.Error("درخواست نامعتبر: داده‌های ورودی موجود نیست");
                }

                // Get existing success story
                var successStory = await _successStoryRepository.GetSuccessStoryByIdAsync(request.Id, cancellationToken);
                if (successStory == null)
                {
                    _logger.LogError("استوری موفقیت با شناسه {Id} یافت نشد", request.Id);
                    return OperationResult.Error("استوری موفقیت یافت نشد");
                }

                // Handle image update
                string? newImageUrl = null;
                if (request.Dto.ImageFile != null)
                {
                    try
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(successStory.ImageUrl))
                        {
                            await _imageService.DeleteImageAsync(successStory.ImageUrl);
                        }

                        // Upload new image
                        newImageUrl = await _imageService.HandleImageUploadAsync(request.Dto.ImageFile);
                        _logger.LogInformation("فایل تصویر جدید با موفقیت آپلود شد: {ImageUrl}", newImageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در آپلود فایل تصویر جدید");
                        return OperationResult.Error($"خطا در آپلود تصویر: {ex.Message}");
                    }
                }

                // Update success story with simplified structure
                try
                {
                    successStory.Update(
                        title: request.Dto.Title,
                        description: request.Dto.Description,
                        imageUrl: newImageUrl ?? request.Dto.ImageUrl,
                        actionLabel: request.Dto.ActionLabel,
                        actionLink: request.Dto.ActionLink,
                        order: request.Dto.Order,
                        isActive: request.Dto.IsActive
                    );
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "خطا در اعتبارسنجی داده‌های به‌روزرسانی استوری موفقیت: {Message}", ex.Message);
                    return OperationResult.Error($"داده‌های ورودی نامعتبر: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در به‌روزرسانی شیء استوری موفقیت");
                    return OperationResult.Error("خطا در به‌روزرسانی استوری موفقیت: داده‌های ورودی نامعتبر");
                }

                // Save changes to database
                try
                {
                    _successStoryRepository.Update(successStory);
                    await _successStoryRepository.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogInformation("استوری موفقیت {Id} با موفقیت به‌روزرسانی شد. عنوان: {Title}", request.Id, successStory.Title);
                    return OperationResult.Success("استوری موفقیت با موفقیت به‌روزرسانی شد");
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _logger.LogError(ex, "خطا در ذخیره تغییرات استوری موفقیت در پایگاه داده. Inner Exception: {InnerException}", ex.InnerException?.Message);
                    return OperationResult.Error("خطا در ذخیره تغییرات: مشکل در پایگاه داده");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطای غیرمنتظره در ذخیره تغییرات استوری موفقیت");
                    return OperationResult.Error("خطا در ذخیره تغییرات");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("عملیات به‌روزرسانی استوری موفقیت لغو شد");
                return OperationResult.Error("عملیات لغو شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره در به‌روزرسانی استوری موفقیت {Id}. Request: {@Request}", request?.Id, new { 
                    Id = request?.Id,
                    Title = request?.Dto?.Title,
                    HasImageFile = request?.Dto?.ImageFile != null
                });
                return OperationResult.Error("خطای غیرمنتظره در به‌روزرسانی استوری موفقیت");
            }
        }
    }
}