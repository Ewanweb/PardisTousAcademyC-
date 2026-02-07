using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostById;

public class GetPostByIdQuery : IRequest<PostDetailDto?>
{
    public Guid Id { get; set; }
    public bool IncludeDrafts { get; set; } = true;
}
