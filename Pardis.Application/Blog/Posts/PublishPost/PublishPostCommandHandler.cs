using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Blog;

namespace Pardis.Application.Blog.Posts.PublishPost;

public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, OperationResult>
{
    private readonly IRepository<Post> _postRepository;

    public PublishPostCommandHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<OperationResult> Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Table.FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);
        if (post == null)
            return OperationResult.NotFound("مطلب یافت نشد");

        post.Publish(request.PublishedAt);
        _postRepository.Update(post);
        await _postRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success("مطلب منتشر شد");
    }
}
