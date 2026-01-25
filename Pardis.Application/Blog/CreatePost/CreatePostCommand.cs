using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MediatR;
using Pardis.Application._Shared;
using Pardis.Domain.Dto.Blog;

namespace Pardis.Application.Blog.CreatePost
{
    public record CreatePostCommand(PostDTO Dto) : IRequest<OperationResult<PostResource>>;
}
