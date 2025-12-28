using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.FileUtil;
using Pardis.Application.Sliders._Shared;
using Pardis.Domain.Sliders;

namespace Pardis.Application.Sliders.HeroSlides.Create
{
    public class CreateHeroSlideCommandHandler : IRequestHandler<CreateHeroSlideCommand, OperationResult>
    {
        private readonly IHeroSlideRepository _heroSlideRepository;
        private readonly IFileService _imageService;
        private readonly ILogger<CreateHeroSlideCommandHandler> _logger;

        public CreateHeroSlideCommandHandler(
            IHeroSlideRepository heroSlideRepository, 
            IFileService imageService,
            ILogger<CreateHeroSlideCommandHandler> logger)
        {
            _heroSlideRepository = heroSlideRepository;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateHeroSlideCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (request?.Dto == null)
                {
                    _logger.LogError("CreateHeroSlideCommand یا Dto آن null است");
                    return OperationResult.Error("درخواست نامعتبر: داده‌های ورودی موجود نیست");
                }

                if (string.IsNullOrWhiteSpace(request.Dto.Title))
                {
                    _logger.LogError("عنوان اسلاید خالی یا null است");
                    return OperationResult.Error("عنوان اسلاید الزامی است");
                }

                // Handle image upload
                string imageUrl = "";
                if (request.Dto.ImageFile != null)
                {
                    try
                    {
                        if (!Directory.Exists(Directories.Slider))
                            Directory.CreateDirectory(Directories.Slider);
                        
                        imageUrl = await _imageService.SaveFileAndGenerateName(request.Dto.ImageFile, Directories.Slider);
                        _logger.LogInformation("فایل تصویر با موفقیت آپلود شد: {ImageUrl}", imageUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در آپلود فایل تصویر");
                        return OperationResult.Error($"خطا در آپلود تصویر: {ex.Message}");
                    }
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

                // Create hero slide with simplified structure
                Domain.Sliders.HeroSlide heroSlide;
                try
                {
                    heroSlide = Domain.Sliders.HeroSlide.Create(
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
                    _logger.LogError(ex, "خطا در اعتبارسنجی داده‌های اسلاید: {Message}", ex.Message);
                    return OperationResult.Error($"داده‌های ورودی نامعتبر: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ایجاد شیء اسلاید");
                    return OperationResult.Error("خطا در ایجاد اسلاید: داده‌های ورودی نامعتبر");
                }

                // Save to database
                try
                {
                    await _heroSlideRepository.AddAsync(heroSlide);
                    await _heroSlideRepository.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogInformation("اسلاید جدید {Id} با موفقیت ایجاد شد. عنوان: {Title}", heroSlide.Id, heroSlide.Title);
                    return OperationResult.Success("اسلاید با موفقیت ایجاد شد");
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _logger.LogError(ex, "خطا در ذخیره اسلاید در پایگاه داده. Inner Exception: {InnerException}", ex.InnerException?.Message);
                    return OperationResult.Error("خطا در ذخیره اسلاید: مشکل در پایگاه داده");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطای غیرمنتظره در ذخیره اسلاید");
                    Console.WriteLine(GetFullException(ex));
                    return OperationResult.Error("خطا در ذخیره اسلاید");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("عملیات ایجاد اسلاید لغو شد");
                return OperationResult.Error("عملیات لغو شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره در ایجاد اسلاید جدید. Request: {@Request}", new { 
                    Title = request?.Dto?.Title, 
                    UserId = request?.CurrentUserId,
                    HasImageFile = request?.Dto?.ImageFile != null
                });
                return OperationResult.Error("خطای غیرمنتظره در ایجاد اسلاید");
            }
        }
        private static string GetFullException(Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            var cur = ex;
            var i = 0;
            while (cur != null)
            {
                sb.AppendLine($"[{i}] {cur.GetType().FullName}: {cur.Message}");
                cur = cur.InnerException;
                i++;
            }
            return sb.ToString();
        }

    }
}