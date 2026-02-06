using MediatR;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostsByTag;

public class GetPostsByTagSlugQuery : IRequest<PagedResult<PostListItemDto>>
{
    public string Slug { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
