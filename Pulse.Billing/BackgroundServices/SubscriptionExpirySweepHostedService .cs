using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Billing.Interfaces;


namespace Pulse.Billing.BackgroundServices;

public class SubscriptionExpirySweepHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval;

    public SubscriptionExpirySweepHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _interval = TimeSpan.FromMinutes(configuration.GetValue<int>("Billing:SweepIntervalMinutes", 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var sweeper = scope.ServiceProvider.GetRequiredService<ISubscriptionExpirySweepService>();
                await sweeper.SweepAsync(stoppingToken);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}