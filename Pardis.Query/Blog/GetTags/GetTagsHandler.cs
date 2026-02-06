using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Query.Blog.GetTags;

public class GetTagsHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    private readonly IRepository<Tag> _tagRepository;

    public GetTagsHandler(IRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        return await _tagRepository.Table
            .AsNoTracking()
            .OrderBy(t => t.Title)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Title = t.Title,
                Slug = t.Slug
            })
            .ToListAsync(cancellationToken);
    }
}
