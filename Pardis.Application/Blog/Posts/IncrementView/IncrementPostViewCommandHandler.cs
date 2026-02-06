using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Blog;

namespace Pardis.Application.Blog.Posts.IncrementView;

public class IncrementPostViewCommandHandler : IRequestHandler<IncrementPostViewCommand, OperationResult>
{
    private readonly IRepository<Post> _postRepository;

    public IncrementPostViewCommandHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<OperationResult> Handle(IncrementPostViewCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Table
            .FirstOrDefaultAsync(p => p.Slug == request.Slug && !p.IsDeleted && p.Status == PostStatus.Published, cancellationToken);

        if (post == null)
            return OperationResult.NotFound("مطلب یافت نشد");

        post.IncrementViews();
        _postRepository.Update(post);
        await _postRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }
}
