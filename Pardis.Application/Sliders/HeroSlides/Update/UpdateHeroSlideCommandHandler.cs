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
        private readonly IFileService _imageService;
        private readonly ILogger<UpdateHeroSlideCommandHandler> _logger;

        public UpdateHeroSlideCommandHandler(
            IHeroSlideRepository heroSlideRepository, 
            IFileService imageService,
            ILogger<UpdateHeroSlideCommandHandler> logger)
        {
            _heroSlideRepository = heroSlideRepository;
            _imageService = imageService;
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
                        // Delete old image
                        if (!string.IsNullOrEmpty(heroSlide.ImageUrl))
                        {
                            _imageService.DeleteFile(Directories.Slider,heroSlide.ImageUrl);
                        }

                        // Upload new image
                        newImageUrl = await _imageService.SaveFileAndGenerateName(request.Dto.ImageFile, Directories.Slider);
                        // ساخت URL کامل برای دسترسی از طریق وب
                        newImageUrl = $"/uploads/sliders/{newImageUrl}";
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