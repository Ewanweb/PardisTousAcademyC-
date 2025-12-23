using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Delete;

namespace Pardis.Infrastructure.Handlers.Sliders.SuccessStories
{
    public class DeleteSuccessStoryCommandHandler : IRequestHandler<DeleteSuccessStoryCommand, OperationResult>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeleteSuccessStoryCommandHandler> _logger;

        public DeleteSuccessStoryCommandHandler(AppDbContext context, ILogger<DeleteSuccessStoryCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var successStory = await _context.SuccessStories
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (successStory == null)
                {
                    return OperationResult.Error("استوری موفقیت یافت نشد");
                }

                // حذف تصویر از فایل سیستم
                if (!string.IsNullOrEmpty(successStory.ImageUrl) && successStory.ImageUrl.StartsWith("/uploads/"))
                {
                    var imagePath = Path.Combine("Uploads", successStory.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        _logger.LogInformation("تصویر استوری موفقیت {ImageUrl} حذف شد", successStory.ImageUrl);
                    }
                }

                _context.SuccessStories.Remove(successStory);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("استوری موفقیت {Id} با موفقیت حذف شد", request.Id);

                return OperationResult.Success("استوری موفقیت با موفقیت حذف شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف استوری موفقیت {Id}", request.Id);
                return OperationResult.Error("خطا در حذف استوری موفقیت");
            }
        }
    }
}