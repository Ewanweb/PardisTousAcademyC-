using Microsoft.EntityFrameworkCore;
using Pardis.Domain.Blog;
using Pardis.Domain;

namespace Pardis.Infrastructure.Repository;

public class PostRepository : Repository<Post>, IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Post?> GetWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Posts
            .Include(p => p.BlogCategory)
            .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
