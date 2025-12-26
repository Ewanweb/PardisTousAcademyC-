using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.HeroSlides.Create;
using Pardis.Domain.Sliders;
using System.Text.Json;

namespace Pardis.Infrastructure.Handlers.Sliders.HeroSlides
{
    public class CreateHeroSlideCommandHandler : IRequestHandler<CreateHeroSlideCommand, OperationResult>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly ILogger<CreateHeroSlideCommandHandler> _logger;

        public CreateHeroSlideCommandHandler(IHeroSlideRepository heroSlideRepository, ILogger<CreateHeroSlideCommandHandler> logger)
        {
            _heroSlideRepository = heroSlideRepository;
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

                // Serialize stats if provided
                string? statsJson = null;
                if (request.Dto.Stats != null && request.Dto.Stats.Count > 0)
                {
                    try
                    {
                        statsJson = JsonSerializer.Serialize(request.Dto.Stats);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "خطا در سریالایز کردن آمار اسلاید");
                    }
                }

                var heroSlide = Domain.Sliders.HeroSlide.Create(
                    title: request.Dto.Title,
                    imageUrl: imageUrl,
                    createdByUserId: userId,
                    description: request.Dto.Description,
                    badge: request.Dto.Badge,
                    primaryActionLabel: request.Dto.PrimaryActionLabel ?? request.Dto.ButtonText,
                    primaryActionLink: request.Dto.PrimaryActionLink ?? request.Dto.LinkUrl,
                    secondaryActionLabel: request.Dto.SecondaryActionLabel,
                    secondaryActionLink: request.Dto.SecondaryActionLink,
                    statsJson: statsJson,
                    order: request.Dto.Order,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent ? null : request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24)
                );

                await _heroSlideRepository.AddAsync(heroSlide);
                await _heroSlideRepository.SaveChangesAsync(cancellationToken);

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