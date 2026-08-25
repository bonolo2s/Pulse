using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Billing.Interfaces;


namespace Pulse.Billing.BackgroundServices;

public class SubscriptionExpirySweepHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // TODO: move to config

    public SubscriptionExpirySweepHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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