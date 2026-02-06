namespace Pardis.Domain.Blog;

public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}
