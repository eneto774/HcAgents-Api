using HcAgents.Application.Setup;
using HcAgents.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HcAgents.Common.Setup;

public static class AppDependencies
{
    public static IServiceCollection AddAppDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddInfrastructure(configuration).AddApplication();
        services.AddJWTAuthentication(configuration).AddSwagger();

        return services;
    }
}
