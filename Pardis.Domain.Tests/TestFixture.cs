using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pardis.Domain.Users;
using Pardis.Infrastructure;
using Pardis.Application.Users.UpdateProfile;
using AutoMapper;
using MediatR;

namespace Pardis.Domain.Tests;

public class TestFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        // Add Entity Framework with In-Memory Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // Add Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Add AutoMapper with proper configuration
        services.AddAutoMapper(cfg => {
            cfg.AddMaps(typeof(UpdateUserProfileHandler).Assembly);
        });

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateUserProfileHandler).Assembly));

        // Add Logging
        services.AddLogging(builder => builder.AddConsole());

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}