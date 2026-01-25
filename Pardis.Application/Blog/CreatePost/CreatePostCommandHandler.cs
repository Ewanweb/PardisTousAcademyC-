using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.ExtensionMappers;
using Pardis.Application.FileUtil;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Users;

namespace Pardis.Application.Blog.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<PostResource>>
{
    private readonly IPostRepository _repository;
    private readonly UserManager<User> _userManager;
    private readonly IFileService _fileService;

    public CreatePostCommandHandler(IPostRepository repository, IFileService fileService, UserManager<User> userManager)
    {
        _repository = repository;
        _fileService = fileService;
        _userManager = userManager;
    }
    public async Task<OperationResult<PostResource>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.Dto.UserId!);

            if (user == null)
                return OperationResult<PostResource>.Error("نویسنده یافت نشد");

            var imageName = await _fileService.SaveFileAndGenerateName(request.Dto.Thumbnail, Directories.Post);

            string imageUrl = $"{Directories.Post}/{imageName}";

            var post = request.Dto.ToEntity();

            post.UserId = user.Id;

            post.Author = user.FullName!;

            post.Thumbnail = imageName;

            post.ThumbnailUrl = imageUrl;

            await _repository.AddAsync(post);

            var save = await _repository.SaveChangesAsync(cancellationToken);

            if (save <= 0)
                return OperationResult<PostResource>.Error("خطا در ذخیره سازی اطلاعات");

            var result = post.ToResource();

            return OperationResult<PostResource>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<PostResource>.Error();
        };
    }
}