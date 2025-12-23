using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Update;

namespace Pardis.Infrastructure.Handlers.Sliders.SuccessStories
{
    public class UpdateSuccessStoryCommandHandler : IRequestHandler<UpdateSuccessStoryCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdateSuccessStoryCommandHandler> _logger;

        public UpdateSuccessStoryCommandHandler(AppDbContext context, ILogger<UpdateSuccessStoryCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var successStory = await _context.SuccessStories
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (successStory == null)
                {
                    return OperationResult.Error("استوری موفقیت یافت نشد");
                }

                // مدیریت تصویر جدید
                string? newImageUrl = null;
                if (request.Dto.ImageFile != null)
                {
                    // حذف تصویر قبلی
                    if (!string.IsNullOrEmpty(successStory.ImageUrl) && successStory.ImageUrl.StartsWith("/uploads/"))
                    {
                        var oldImagePath = Path.Combine("Uploads", successStory.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    // ذخیره تصویر جدید
                    var uploadsPath = Path.Combine("Uploads", "sliders", "stories");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Dto.ImageFile.CopyToAsync(stream, cancellationToken);
                    }

                    newImageUrl = $"/uploads/sliders/stories/{fileName}";
                }

                // به‌روزرسانی فیلدها
                successStory.Update(
                    title: request.Dto.Title,
                    description: request.Dto.Description,
                    imageUrl: newImageUrl ?? request.Dto.ImageUrl,
                    studentName: request.Dto.StudentName,
                    courseName: request.Dto.CourseName,
                    linkUrl: request.Dto.LinkUrl,
                    courseId: request.Dto.CourseId,
                    order: request.Dto.Order,
                    isActive: request.Dto.IsActive,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent == false ? request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24) : null
                );

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("استوری موفقیت {Id} با موفقیت به‌روزرسانی شد", request.Id);

                return OperationResult.Success("استوری موفقیت با موفقیت به‌روزرسانی شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در به‌روزرسانی استوری موفقیت {Id}", request.Id);
                return OperationResult.Error("خطا در به‌روزرسانی استوری موفقیت");
            }
        }
    }
}