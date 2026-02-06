using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetRelatedPosts;

public class GetRelatedPostsQuery : IRequest<List<PostListItemDto>>
{
    public string Slug { get; set; } = string.Empty;
    public int Take { get; set; } = 6;
}
