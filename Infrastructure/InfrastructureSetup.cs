using HcAgents.Domain.Abstractions;
using HcAgents.Infrastructure.Cache;
using HcAgents.Infrastructure.Database;
using HcAgents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HcAgents.Infrastructure.Setup;

public static class InfrastructureSetup
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString));
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMemoryCache();

        services.AddScoped<IMemoryCacheRepository, MemoryCacheRepository>();

        return services;
    }
}
