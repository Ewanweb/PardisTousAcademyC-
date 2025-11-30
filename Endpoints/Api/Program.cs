using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Pardis.Infrastructure;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. ناحیه تعریف سرویس‌ها (DI Container)
// =========================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// حذف AddOpenApi (چون از Swashbuckle استفاده می‌کنیم)
// حذف AddSwaggerGen خالی (تکراری)

// کانفیگ Swagger با JWT
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Pardis API", Version = "v1" });

    // الف) تعریف امنیت
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // ب) اعمال امنیت
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
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
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Seeding Data: {ex.Message}");
    }
}

// =========================================================
// 3. ناحیه تنظیمات پایپ‌لاین (Middleware Pipeline)
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization(); // ذخیره توکن بعد از رفرش
    });
    // app.MapOpenApi(); // حذف شد چون تداخل ایجاد می‌کرد
}

app.UseHttpsRedirection(); // بهتر است اینجا باشد
app.UseStaticFiles();

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");

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