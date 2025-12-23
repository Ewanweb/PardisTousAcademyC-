using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Create;

namespace Pardis.Infrastructure.Handlers.Sliders.HeroSlides
{
    public class CreateHeroSlideCommandHandler : IRequestHandler<CreateHeroSlideCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateHeroSlideCommandHandler> _logger;

        public CreateHeroSlideCommandHandler(AppDbContext context, ILogger<CreateHeroSlideCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // مدیریت آپلود تصویر
                string imageUrl = request.Dto.ImageUrl ?? "";
                if (request.Dto.ImageFile != null)
                {
                    var uploadsPath = Path.Combine("Uploads", "sliders", "hero");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Dto.ImageFile.CopyToAsync(stream, cancellationToken);
                    }

                    imageUrl = $"/uploads/sliders/hero/{fileName}";
                }

                var userId = Guid.TryParse(request.CurrentUserId, out var parsedUserId) ? parsedUserId : Guid.Empty;

                var heroSlide = Domain.Sliders.HeroSlide.Create(
                    title: request.Dto.Title,
                    imageUrl: imageUrl,
                    createdByUserId: userId,
                    description: request.Dto.Description,
                    linkUrl: request.Dto.LinkUrl,
                    buttonText: request.Dto.ButtonText,
                    order: request.Dto.Order,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent ? null : request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24)
                );

                _context.HeroSlides.Add(heroSlide);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("اسلاید جدید {Id} با موفقیت ایجاد شد", heroSlide.Id);

                return OperationResult.Success("اسلاید با موفقیت ایجاد شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد اسلاید جدید");
                return OperationResult.Error("خطا در ایجاد اسلاید");
            }
        }
    }
}