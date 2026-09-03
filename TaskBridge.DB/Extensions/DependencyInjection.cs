using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TaskBridge.Application.Abstractions.Persistence;

namespace TaskBridge.DB.Extensions;

/// <summary>
/// Contains database dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers PostgreSQL persistence services.
    /// </summary>
    public static IServiceCollection AddTaskBridgeDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskBridge")
            ?? throw new InvalidOperationException(
                "Connection string 'TaskBridgeDatabase' is not configured.");

        //services.AddDbContext<AppDbContext>(options =>
        //    options.UseNpgsql(
        //        connectionString,
        //        npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IAppDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<AppDbContext>());

        return services;
    }
}
