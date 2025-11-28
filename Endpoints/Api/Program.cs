using Pardis.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // معمولا برای سواگر لازم است
builder.Services.AddSwaggerGen(); // اگر از Swashbuckle استفاده می‌کنید
builder.Services.Inject(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // این خط مسئول ساختن نقش‌هاست
        await Pardis.Infrastructure.RoleSeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

app.Run();
