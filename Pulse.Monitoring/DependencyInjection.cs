using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Monitoring.DataAccess;
using Pulse.Monitoring.Interfaces;
using Pulse.Monitoring.Services;

namespace Pulse.Monitoring;

public static class DependencyInjection
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MonitoringDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IMonitoringService, MonitoringService>();

        return services;
    }
}