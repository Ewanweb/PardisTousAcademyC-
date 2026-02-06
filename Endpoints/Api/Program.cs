using Api.Authorization;
using Api.Middleware;
using Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Pardis.Domain.Service;
using Pardis.Domain.Users;
using Pardis.Infrastructure;
using Pardis.Infrastructure.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/pardis-academy-.txt", 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
// Use Serilog
builder.Host.UseSerilog();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext, RequestContext>();
// =========================================================
// 1. ناحیه تعریف سرویس‌ها (DI Container)
// =========================================================

builder.Services.AddControllers(options =>
    {
        // اضافه کردن فیلتر اعتبارسنجی مدل‌ها
        options.Filters.Add<Api.Filters.ModelValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        // ✅ Handle circular references
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        
        // ✅ Convert property names to camelCase (paymentMethod -> PaymentMethod)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        
        // ✅ Convert enum values from strings (e.g., "Manual", "Online" -> PaymentMethod enum)
        // Enum names should match exactly: "Manual", "Online", "Wallet", etc.
        // allowIntegerValues: true allows both string names and integer values (0, 1, 2, ...)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(
            namingPolicy: null, // Use exact enum names (PascalCase)
            allowIntegerValues: true));
    });

builder.Services.AddEndpointsApiExplorer();

// ✅ Add Authorization with Pardis Policies
builder.Services.AddAuthorization(options => options.AddPardisPolicies());

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName!.Replace("+", "."));

    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Pardis Academy API",
        Description = "API Documentation for Pardis Academy",
        Contact = new OpenApiContact()
        {
            Email = "pardistous.ir@gmail.com",
            Name = "Pardis Academy",
            Url = new Uri("https://pardistous.ir")
        }
    });

    // ✅ تنظیمات صحیح JWT برای Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    // ✅ اعمال Security به تمام APIها
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // ✅ اضافه کردن XML Comments برای بهتر شدن documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// کانفیگ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});

// تزریق سرویس‌های لایه زیرساخت
builder.Services.Inject(builder.Configuration);

// ثبت MediatR handlers
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Pardis.Query.Categories.GetCategories.GetCategoriesQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Pardis.Application.Shopping.Cart.AddCourseToCart.AddCourseToCartCommand).Assembly);
});

var app = builder.Build();

// =========================================================
// 2. ناحیه اجرای Seeder (ساخت نقش‌ها و ادمین اولیه)
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await Pardis.Infrastructure.RoleSeeder.SeedAsync(services);
        await Pardis.Infrastructure.UserSeeder.SeedAsync(services);
        
        // Seed sample system logs
        var dbContext = services.GetRequiredService<Pardis.Infrastructure.AppDbContext>();
        await Pardis.Infrastructure.SystemLogSeeder.SeedSampleLogs(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Seeding Data: {ex.Message}");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// =========================================================
// 3. ناحیه تنظیمات پایپ‌لاین (Middleware Pipeline)
// =========================================================

// اضافه کردن میدل‌ویر مدیریت خطاهای سراسری
app.UseMiddleware<GlobalExceptionMiddleware>();

// ✅ اضافه کردن middleware برای debug authentication (فقط در development)
if (app.Environment.IsDevelopment())
{
    app.UseAuthenticationDebug();
}


#region Swagger

app.UseSwagger();
app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


#endregion

app.UseHttpsRedirection(); // بهتر است اینجا باشد
app.UseStaticFiles();

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();