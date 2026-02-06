using MediatR;
using Pardis.Application._Shared.Pagination;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostsList;

public class GetPostsListQuery : IRequest<PagedResult<PostListItemDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string Sort { get; set; } = "newest";
    public string? CategorySlug { get; set; }
    public string? TagSlug { get; set; }
    public string? Q { get; set; }
    public string? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool IncludeDrafts { get; set; }
}
