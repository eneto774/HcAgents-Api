using HcAgents.Application.Services;
using HcAgents.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HcAgents.Application;

public static class ApplicationSetup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IJwtService, JwtService>();

        var serviceHandlers = AppDomain.CurrentDomain.Load("Application");
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(serviceHandlers);
        });

        return services;
    }
}
