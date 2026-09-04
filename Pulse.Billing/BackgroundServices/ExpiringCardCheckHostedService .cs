using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.BackgroundServices;

public class ExpiringCardCheckHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval;

    public ExpiringCardCheckHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _interval = TimeSpan.FromDays(configuration.GetValue<int>("Billing:ExpiringCardCheckIntervalDays", 1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var checker = scope.ServiceProvider.GetRequiredService<IExpiringCardCheckService>();
                await checker.CheckAsync(stoppingToken);
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }
}