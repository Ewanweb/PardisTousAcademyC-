using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetPostNavigation;

public class GetPostNavigationQuery : IRequest<PostNavDto>
{
    public string Slug { get; set; } = string.Empty;
}
