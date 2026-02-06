using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Application.Sliders._Shared;
using Pardis.Domain.Sliders;

namespace Pardis.Application.Sliders.HeroSlides.Update
{
    public class UpdateHeroSlideCommandHandler : IRequestHandler<UpdateHeroSlideCommand, OperationResult>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly ISecureFileService _secureFileService;
        private readonly ILogger<UpdateHeroSlideCommandHandler> _logger;

        public UpdateHeroSlideCommandHandler(
            IHeroSlideRepository heroSlideRepository, 
            ISecureFileService secureFileService,
            ILogger<UpdateHeroSlideCommandHandler> logger)
        {
            _heroSlideRepository = heroSlideRepository;
            _secureFileService = secureFileService;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var heroSlide = await _heroSlideRepository.GetHeroSlideByIdAsync(request.Id, cancellationToken);

                if (heroSlide == null)
                {
                    return OperationResult.Error("اسلاید یافت نشد");
                }

                // Handle image update
                string? newImageUrl = null;
                if (request.Dto.ImageFile != null)
                {
                    try
                    {
                        var uploadResult = await _secureFileService.SaveFileSecurely(
                            request.Dto.ImageFile,
                            "sliders",
                            request.CurrentUserId
                        );

                        if (!uploadResult.IsSuccess)
                        {
                            _logger.LogError("خطا در آپلود تصویر جدید: {Error}", uploadResult.ErrorMessage);
                            return OperationResult.Error($"خطا در آپلود تصویر: {uploadResult.ErrorMessage}");
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(heroSlide.ImageUrl))
                        {
                            try
                            {
                                if (File.Exists($"{Directories.Slider}/{heroSlide.ImageUrl}"))
                                {
                                    File.Delete($"{Directories.Slider}/{heroSlide.ImageUrl}");
                                }
                            }
                            catch (Exception deleteEx)
                            {
                                _logger.LogWarning(deleteEx, "خطا در حذف تصویر قدیمی");
                            }
                        }

                        // ساخت URL کامل برای دسترسی از طریق وب
                        newImageUrl = $"/uploads/sliders/{uploadResult.SecureFileName}";
                        _logger.LogInformation("تصویر جدید با موفقیت آپلود شد: {ImageUrl}", newImageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در آپلود تصویر جدید");
                        return OperationResult.Error($"خطا در آپلود تصویر: {ex.Message}");
                    }
                }

                // Update hero slide with simplified structure
                heroSlide.Update(
                    title: request.Dto.Title,
                    description: request.Dto.Description,
                    imageUrl: newImageUrl,
                    actionLabel: request.Dto.ActionLabel,
                    actionLink: request.Dto.ActionLink,
                    order: request.Dto.Order,
                    isActive: request.Dto.IsActive
                );

                _heroSlideRepository.Update(heroSlide);
                await _heroSlideRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("اسلاید {Id} با موفقیت به‌روزرسانی شد", request.Id);

                return OperationResult.Success("اسلاید با موفقیت به‌روزرسانی شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در به‌روزرسانی اسلاید {Id}", request.Id);
                return OperationResult.Error("خطا در به‌روزرسانی اسلاید");
            }
        }
    }
}