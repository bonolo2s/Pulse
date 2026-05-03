using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pulse.StatusPages.DataAccess;
using Pulse.StatusPages.Interfaces;
using Pulse.StatusPages.Services;

namespace Pulse.StatusPages;

public static class DependencyInjection
{
    public static IServiceCollection AddStatusPages(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StatusPagesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IStatusPageService, StatusPageService>();

        return services;
    }
}