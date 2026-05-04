using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Services;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing;

public static class DependencyInjection
{
    public static IServiceCollection AddBilling(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IBillingValidator, BillingService>();
        services.AddScoped<ISubscriptionCreator, BillingService>();

        return services;
    }
}