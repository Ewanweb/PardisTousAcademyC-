using MediatR;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetTags;

public class GetTagsQuery : IRequest<List<TagDto>>
{
}
