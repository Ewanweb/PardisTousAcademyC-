using MediatR;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.SearchPosts;

public class SearchPostsQuery : IRequest<PagedResult<PostListItemDto>>
{
    public string Q { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
