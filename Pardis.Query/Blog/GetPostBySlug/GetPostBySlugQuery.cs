using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostBySlug;

public class GetPostBySlugQuery : IRequest<PostSlugResolveDto>
{
    public string Slug { get; set; } = string.Empty;
    public bool IncludeDrafts { get; set; }
}
