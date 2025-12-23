using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Create;

namespace Pardis.Infrastructure.Handlers.Sliders.SuccessStories
{
    public class CreateSuccessStoryCommandHandler : IRequestHandler<CreateSuccessStoryCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateSuccessStoryCommandHandler> _logger;

        public CreateSuccessStoryCommandHandler(AppDbContext context, ILogger<CreateSuccessStoryCommandHandler> logger)
        {
            _context = context;
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
                    description: request.Dto.Description,
                    studentName: request.Dto.StudentName,
                    courseName: request.Dto.CourseName,
                    linkUrl: request.Dto.LinkUrl,
                    courseId: request.Dto.CourseId,
                    order: request.Dto.Order,
                    isPermanent: request.Dto.IsPermanent,
                    expiresAt: request.Dto.IsPermanent ? null : request.Dto.ExpiresAt ?? DateTime.UtcNow.AddHours(24)
                );

                _context.SuccessStories.Add(successStory);
                await _context.SaveChangesAsync(cancellationToken);

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