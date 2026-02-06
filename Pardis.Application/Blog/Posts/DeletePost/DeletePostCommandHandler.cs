using MediatR;
using Microsoft.EntityFrameworkCore;
using Pardis.Application._Shared;
using Pardis.Domain;
using Pardis.Domain.Blog;

namespace Pardis.Application.Blog.Posts.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, OperationResult>
{
    private readonly IRepository<Post> _postRepository;

    public DeletePostCommandHandler(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<OperationResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Table.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
        if (post == null)
            return OperationResult.NotFound("مطلب یافت نشد");

        post.MarkDeleted();
        _postRepository.Update(post);
        await _postRepository.SaveChangesAsync(cancellationToken);

        return OperationResult.Success("مطلب حذف شد");
    }
}
