using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pardis.Application._Shared;
using Pardis.Application._Shared.JWT;
using Pardis.Application.Categories.Update.Pardis.Application.Categories.Commands;
using Pardis.Application.FileUtil;
using Pardis.Application.Payments.Contracts;
using Pardis.Application.Shopping.Contracts;
using Pardis.Application.Sliders._Shared;
using Pardis.Domain;
using Pardis.Domain.Courses;
using Pardis.Domain.Users;
using Pardis.Domain.Payments;
using Pardis.Domain.Attendance;
using Pardis.Domain.Sliders;
using Pardis.Domain.Settings;
using Pardis.Infrastructure.Repository;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;


namespace Pardis.Infrastructure
{
    public static class ApplicationBootstrapper
    {
        public static IServiceCollection Inject(this IServiceCollection service, IConfiguration config)
        {

            // 1. دیتابیس
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    sql => sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    ));
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
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 3. تنظیمات JWT
            var jwtSettings = config.GetSection("JwtSettings");
            var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = Encoding.UTF8.GetBytes(jwtKey);

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // برای development
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "PardisAcademy",
                    ValidAudience = jwtSettings["Audience"] ?? "PardisAcademyUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromMinutes(5), // اجازه 5 دقیقه تفاوت زمانی
                    RequireExpirationTime = true,
                    ValidateTokenReplay = false
                };
                
                // ✅ بهبود Event Handlers برای debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                        
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        else if (context.Exception.GetType() == typeof(SecurityTokenValidationException))
                        {
                            context.Response.Headers["Token-Invalid"] = "true";
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"JWT Token validated successfully for user: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"JWT Challenge: {context.Error} - {context.ErrorDescription}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        if (!string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine($"JWT Token received: {token.Substring(0, Math.Min(20, token.Length))}...");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            service.AddAutoMapper(cfg => 
            {
                cfg.AddProfile<MappingProfile>();
            });

            service.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UpdateCategoryHandler).Assembly);
                // Query handlers will be registered separately in the API project
            });

            // 5. تزریق وابستگی‌های کاستوم
            service.AddScoped<IFileService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var webHostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
                
                // Set the root path to wwwroot for file storage
                var tempConfig = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["FileStorage:RootPath"] = webHostEnvironment.WebRootPath
                    })
                    .Build();
                
                return new FileService(tempConfig);
            });
            service.AddScoped<ITokenService, TokenService>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<ICategoryRepository, CategoryRepository>();
            service.AddScoped<ICourseRepository, CourseRepository>();
            service.AddScoped<Pardis.Application.Courses.Contracts.ICourseRepository, ApplicationCourseRepository>();
            service.AddScoped<TransactionRepository>();
            service.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();
            service.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
            service.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
            service.AddScoped<IStudentAttendanceRepository, StudentAttendanceRepository>();
            service.AddScoped<ICourseScheduleRepository, CourseScheduleRepository>();
            
            // Slider Repositories
            service.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
            service.AddScoped<ISuccessStoryRepository, SuccessStoryRepository>();
            
            // Slider Services
            service.AddScoped<ISliderImageService, SliderImageService>();
            
            // Shopping Repositories
            service.AddScoped<ICartRepository, CartRepository>();
            service.AddScoped<IOrderRepository, OrderRepository>();
            service.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
            
            // Payment Repositories (existing)
            service.AddScoped<IEnrollmentRepository, CourseEnrollmentRepository>();

            // ریپازیتوری جنریک
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));




            return service;
        }
    }
}
