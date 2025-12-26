using MediatR;
using Microsoft.Extensions.Logging;
using Pardis.Application._Shared;
using Pardis.Application.Sliders.SuccessStories.Delete;
using Pardis.Domain.Sliders;

namespace Pardis.Infrastructure.Handlers.Sliders.SuccessStories
{
    public class DeleteSuccessStoryCommandHandler : IRequestHandler<DeleteSuccessStoryCommand, OperationResult>
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly ILogger<DeleteSuccessStoryCommandHandler> _logger;

        public DeleteSuccessStoryCommandHandler(ISuccessStoryRepository successStoryRepository, ILogger<DeleteSuccessStoryCommandHandler> logger)
        {
            _successStoryRepository = successStoryRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteSuccessStoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var successStory = await _successStoryRepository.GetSuccessStoryByIdAsync(request.Id, cancellationToken);

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

                _successStoryRepository.Remove(successStory);
                await _successStoryRepository.SaveChangesAsync(cancellationToken);

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