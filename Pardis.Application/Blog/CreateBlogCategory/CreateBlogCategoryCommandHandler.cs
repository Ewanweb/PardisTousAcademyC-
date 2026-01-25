using MediatR;
using Microsoft.AspNetCore.Identity;
using Pardis.Application._Shared;
using Pardis.Application._Shared.ExtensionMappers;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Blog;
using Pardis.Domain.Dto.Blog;
using Pardis.Domain.Users;

namespace Pardis.Application.Blog.CreateBlogCategory;

public class CreateBlogCategoryCommandHandler : IRequestHandler<CreateBlogCategoryCommand, OperationResult<BlogCategoriesResource>>
{
    private readonly IRepository<BlogCategory> _repository;
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;

    public CreateBlogCategoryCommandHandler(IRepository<BlogCategory> repository, IFileService fileService, UserManager<User> userManager)
    {
        _repository = repository;
        _fileService = fileService;
        _userManager = userManager;
    }

    public async Task<OperationResult<BlogCategoriesResource>> Handle(CreateBlogCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.Dto.UserId);

            if (user == null)
                return OperationResult<BlogCategoriesResource>.Error("کاربری با این مشخصات یافت نشد");

            var imageName = await _fileService.SaveFileAndGenerateName(request.Dto.Thumbnail, Directories.BlogCategory);

            var imageUrl = $"{Directories.BlogCategory}/{imageName}";

            var category = request.Dto.ToEntity();

            category.Thumbnail = imageName;

            category.UserId = request.Dto.UserId;

            category.ThumbnailUrl = imageUrl;

            category.CreatedBy = user.FullName!;

            await _repository.AddAsync(category);

            var save = await _repository.SaveChangesAsync(cancellationToken);

            if (save <= 0)
                return OperationResult<BlogCategoriesResource>.Error("خطا در ذخیره سازی اطلاعات");

            var result = category.ToResource();

            return OperationResult<BlogCategoriesResource>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult<BlogCategoriesResource>.Error();
        }

    }
}