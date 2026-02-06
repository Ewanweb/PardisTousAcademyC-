using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Blog;

namespace Pardis.Application.Blog.Tags.DeleteTag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, OperationResult>
{
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<PostTag> _postTagRepository;

    public DeleteTagCommandHandler(IRepository<Tag> tagRepository, IRepository<PostTag> postTagRepository)
    {
        _tagRepository = tagRepository;
        _postTagRepository = postTagRepository;
    }

    public async Task<OperationResult> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.Table.FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);
        if (tag == null)
            return OperationResult.NotFound("تگ یافت نشد");

        var relations = await _postTagRepository.Table.Where(pt => pt.TagId == tag.Id).ToListAsync(cancellationToken);
        if (relations.Count > 0)
            _postTagRepository.RemoveRange(relations);

        await _tagRepository.DeleteAsync(tag);
        await _tagRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success("تگ حذف شد");
    }
}
