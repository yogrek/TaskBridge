using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.DB;

using System.Text;

namespace TaskBridge.ApiTests.Infrastructure;

public sealed class TaskBridgeApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<IServiceCollection>? _configureTestServices;

    public TaskBridgeApiFactory(
        string connectionString,
        Action<IServiceCollection>? configureTestServices = null)
    {
        _connectionString = connectionString;
        _configureTestServices = configureTestServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TaskBridge"] = _connectionString,
                ["Jwt:Issuer"] = TestJwtTokenFactory.Issuer,
                ["Jwt:Audience"] = TestJwtTokenFactory.Audience,
                ["Jwt:SigningKey"] = TestJwtTokenFactory.SigningKey,
                ["Jwt:ExpirationMinutes"] = "30"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IAppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_connectionString));
            services.AddScoped<IAppDbContext>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = TestJwtTokenFactory.Issuer,
                    ValidateAudience = true,
                    ValidAudience = TestJwtTokenFactory.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(TestJwtTokenFactory.SigningKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            _configureTestServices?.Invoke(services);
        });
    }
}
