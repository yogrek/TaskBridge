using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Infrastructure.Security;

namespace TaskBridge.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAccessTokenProvider, JwtAccessTokenProvider>();

        return services;
    }
}
