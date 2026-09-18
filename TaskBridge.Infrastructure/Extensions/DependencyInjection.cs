using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Infrastructure.Security;
using TaskBridge.Infrastructure.Time;
using TaskBridge.Application.Abstractions.Time;

namespace TaskBridge.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAccessTokenProvider, JwtAccessTokenProvider>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
