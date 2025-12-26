using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Create;
using Pardis.Domain.Sliders;

namespace Pardis.Infrastructure.Handlers.Sliders.SuccessStories
{
    public class CreateSuccessStoryCommandHandler : IRequestHandler<CreateSuccessStoryCommand, OperationResult>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly ILogger<CreateSuccessStoryCommandHandler> _logger;

        public CreateSuccessStoryCommandHandler(ISuccessStoryRepository successStoryRepository, ILogger<CreateSuccessStoryCommandHandler> logger)
        {
            _successStoryRepository = successStoryRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // مدیریت آپلود تصویر
                string imageUrl = request.Dto.ImageUrl ?? "";
                if (request.Dto.ImageFile != null)
                {
                    var uploadsPath = Path.Combine("Uploads", "sliders", "stories");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Dto.ImageFile.CopyToAsync(stream, cancellationToken);
                    }

                    imageUrl = $"/uploads/sliders/stories/{fileName}";
                }

                var userId = Guid.TryParse(request.CurrentUserId, out var parsedUserId) ? parsedUserId : Guid.Empty;

                var successStory = Domain.Sliders.SuccessStory.Create(
                    title: request.Dto.Title,
                    imageUrl: imageUrl,
                    createdByUserId: userId,
                    subtitle: request.Dto.Subtitle,
                    description: request.Dto.Description,
                    badge: request.Dto.Badge,
                    type: request.Dto.Type,
                    studentName: request.Dto.StudentName,
                    courseName: request.Dto.CourseName,
                    actionLabel: request.Dto.ActionLabel,
                    actionLink: request.Dto.ActionLink ?? request.Dto.LinkUrl,
                    statsJson: request.Dto.Stats != null ? System.Text.Json.JsonSerializer.Serialize(request.Dto.Stats) : null,
                    duration: request.Dto.Duration,
                    courseId: request.Dto.CourseId,
                    order: request.Dto.Order,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent ? null : request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24)
                );

                await _successStoryRepository.AddAsync(successStory);
                await _successStoryRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("استوری موفقیت جدید {Id} با موفقیت ایجاد شد", successStory.Id);

                return OperationResult.Success("استوری موفقیت با موفقیت ایجاد شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد استوری موفقیت جدید");
                return OperationResult.Error("خطا در ایجاد استوری موفقیت");
            }
        }
    }
}