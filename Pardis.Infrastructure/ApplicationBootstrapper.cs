using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Application.Categories.Update;
using Pardis.Application.Categories.Update.Pardis.Application.Categories.Commands;
using Pardis.Application.Courses.Create;
using Pardis.Application.FileUtil;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Pardis.Infrastructure.Repository; // اگر کلاس Repository اینجاست
using Pardis.Query.Users.GetUserById;
using System;
using System.Text;

namespace Pardis.Infrastructure
{
    public static class ApplicationBootstrapper
    {
        public static IServiceCollection Inject(this IServiceCollection service, IConfiguration config)
        {
            // 1. دیتابیس
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });


            // 2. تنظیمات Identity
            service.AddIdentity<User, Role>(options =>
            {
                // تنظیمات پسورد
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // تنظیمات کاربر
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 3. تنظیمات JWT
            var jwtSettings = config.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            service.AddAutoMapper(cfg => {}, typeof(MappingProfile).Assembly);

            service.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UpdateCategoryHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetUserByIdQuery).Assembly);
            });

            // 5. تزریق وابستگی‌های کاستوم
            service.AddScoped<IFileService, FileService>();
            service.AddScoped<ITokenService, TokenService>(); // فقط یکبار اینجا باشد کافیست
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<ICategoryRepository, CategoryRepository>();
            service.AddScoped<ICourseRepository, CourseRepository>();

            // ریپازیتوری جنریک
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            return service;
        }
    }
}