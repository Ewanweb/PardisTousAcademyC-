using Pardis.Domain;
using Pardis.Domain.Categories;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Infrastructure.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetCategoryWithIdWithSeo(Guid id, CancellationToken token);
        Task<List<Category>?> GetCategories();
        Task<Category?> GetCategoryById(Guid id, CancellationToken token);
        Task<Category?> GetCategoryChildren(Guid id, CancellationToken token);
        Task<List<CategoryWithCountDto>> GetChildrenWithCourseCountAsync(Guid parentId, CancellationToken token);

        Task<int> MoveCategoryForDelete(Guid id, Guid migrateToId,CancellationToken token);
        Task<bool> IsExist(string title);
        Task<bool> SlugIsExist(string slug);
    }
}