using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Update;

namespace Pardis.Infrastructure.Handlers.Sliders.HeroSlides
{
    public class UpdateHeroSlideCommandHandler : IRequestHandler<UpdateHeroSlideCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdateHeroSlideCommandHandler> _logger;

        public UpdateHeroSlideCommandHandler(AppDbContext context, ILogger<UpdateHeroSlideCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var heroSlide = await _context.HeroSlides
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (heroSlide == null)
                {
                    return OperationResult.Error("اسلاید یافت نشد");
                }

                // مدیریت تصویر جدید
                string? newImageUrl = null;
                if (request.Dto.ImageFile != null)
                {
                    // حذف تصویر قبلی
                    if (!string.IsNullOrEmpty(heroSlide.ImageUrl) && heroSlide.ImageUrl.StartsWith("/uploads/"))
                    {
                        var oldImagePath = Path.Combine("Uploads", heroSlide.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    // ذخیره تصویر جدید
                    var uploadsPath = Path.Combine("Uploads", "sliders", "hero");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Dto.ImageFile.CopyToAsync(stream, cancellationToken);
                    }

                    newImageUrl = $"/uploads/sliders/hero/{fileName}";
                }

                // به‌روزرسانی فیلدها
                heroSlide.Update(
                    title: request.Dto.Title,
                    description: request.Dto.Description,
                    imageUrl: newImageUrl ?? request.Dto.ImageUrl,
                    linkUrl: request.Dto.LinkUrl,
                    buttonText: request.Dto.ButtonText,
                    order: request.Dto.Order,
                    isActive: request.Dto.IsActive,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent == false ? request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24) : null
                );

                await _context.SaveChangesAsync(cancellationToken);

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