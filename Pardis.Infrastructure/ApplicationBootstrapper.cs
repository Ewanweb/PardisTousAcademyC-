using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardis.Application.Courses.Create;
using Pardis.Application.FileUtil;

namespace Pardis.Infrastructure
{
    public static class ApplicationBootstrapper
    {
        public static IServiceCollection Inject(this IServiceCollection service, IConfiguration config)
        {
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            service.AddScoped<IFileService, FileService>();
            
            service.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateCourseCommandHandler).Assembly);
            });
            return service;
        }
    }
}
