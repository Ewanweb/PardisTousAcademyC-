using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Blog;

namespace Pardis.Application.Blog.Categories.DeleteCategory;

public class DeleteBlogCategoryCommandHandler : IRequestHandler<DeleteBlogCategoryCommand, OperationResult>
{
    private readonly IRepository<BlogCategory> _categoryRepository;

    public DeleteBlogCategoryCommandHandler(IRepository<BlogCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<OperationResult> Handle(DeleteBlogCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Table.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (category == null)
            return OperationResult.NotFound("دسته‌بندی یافت نشد");

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success("دسته‌بندی حذف شد");
    }
}
