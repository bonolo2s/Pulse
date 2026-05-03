using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Observability.DataAccess;
using Pulse.Observability.Interfaces;
using Pulse.Observability.Services;

namespace Pulse.Observability;

public static class DependencyInjection
{
    public static IServiceCollection AddObservability(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ObservabilityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IObservabilityService, ObservabilityService>();

        return services;
    }
}