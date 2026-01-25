using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Domain.Blog;

namespace Pardis.Infrastructure.Repository
{
    public class PostRepository : Repository<Post>,IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context)
        {
        }
    }
}
