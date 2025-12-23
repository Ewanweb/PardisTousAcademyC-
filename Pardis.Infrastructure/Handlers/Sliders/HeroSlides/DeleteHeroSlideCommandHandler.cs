using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Delete;

namespace Pardis.Infrastructure.Handlers.Sliders.HeroSlides
{
    public class DeleteHeroSlideCommandHandler : IRequestHandler<DeleteHeroSlideCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeleteHeroSlideCommandHandler> _logger;

        public DeleteHeroSlideCommandHandler(AppDbContext context, ILogger<DeleteHeroSlideCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var heroSlide = await _context.HeroSlides
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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

                _context.HeroSlides.Remove(heroSlide);
                await _context.SaveChangesAsync(cancellationToken);

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