using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Delete;
using Pardis.Domain.Sliders;

namespace Pardis.Infrastructure.Handlers.Sliders.HeroSlides
{
    public class DeleteHeroSlideCommandHandler : IRequestHandler<DeleteHeroSlideCommand, OperationResult>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly ILogger<DeleteHeroSlideCommandHandler> _logger;

        public DeleteHeroSlideCommandHandler(IHeroSlideRepository heroSlideRepository, ILogger<DeleteHeroSlideCommandHandler> logger)
        {
            _heroSlideRepository = heroSlideRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var heroSlide = await _heroSlideRepository.GetHeroSlideByIdAsync(request.Id, cancellationToken);

                if (heroSlide == null)
                {
                    return OperationResult.Error("اسلاید یافت نشد");
                }

                // حذف تصویر از فایل سیستم
                if (!string.IsNullOrEmpty(heroSlide.ImageUrl) && heroSlide.ImageUrl.StartsWith("/uploads/"))
                {
                    var imagePath = Path.Combine("Uploads", heroSlide.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        _logger.LogInformation("تصویر اسلاید {ImageUrl} حذف شد", heroSlide.ImageUrl);
                    }
                }

                _heroSlideRepository.Remove(heroSlide);
                await _heroSlideRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("اسلاید {Id} با موفقیت حذف شد", request.Id);

                return OperationResult.Success("اسلاید با موفقیت حذف شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف اسلاید {Id}", request.Id);
                return OperationResult.Error("خطا در حذف اسلاید");
            }
        }
    }
}