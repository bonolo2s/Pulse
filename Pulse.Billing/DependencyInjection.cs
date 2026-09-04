using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Billing.BackgroundServices;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;
using Pulse.Billing.Payments.Paystack;
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
        services.AddHttpClient<IPaymentProvider, PaystackPaymentProvider>();
        services.AddScoped<IBillingEventWriter, BillingEventWriter>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISubscriptionCreator, SubscriptionService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddScoped<ISubscriptionExpirySweepService, SubscriptionExpirySweepService>();
        services.AddHostedService<SubscriptionExpirySweepHostedService>();
        services.AddScoped<ISubscriptionRenewalSweepService, SubscriptionRenewalSweepService>();
        services.AddHostedService<SubscriptionRenewalSweepHostedService>();
        services.AddScoped<IExpiringCardCheckService, ExpiringCardCheckService>();
        services.AddHostedService<ExpiringCardCheckHostedService>();

        return services;
    }
}