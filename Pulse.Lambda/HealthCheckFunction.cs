using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Lambda.Interfaces;
using Pulse.Lambda.Models;
using Pulse.Shared.DTOs;
using Pulse.Shared.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Pulse.Lambda;

public class HealthCheckFunction
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ISnsPublisher _snsPublisher;
    private readonly IEndpointRepository _endpointRepository;

    public HealthCheckFunction()
    {
        var services = new ServiceCollection();

        services.AddLambdaServices();

        var serviceProvider = services.BuildServiceProvider();

        _healthCheckService = serviceProvider.GetRequiredService<IHealthCheckService>();
        _snsPublisher = serviceProvider.GetRequiredService<ISnsPublisher>();
        _endpointRepository = serviceProvider.GetRequiredService<IEndpointRepository>();
    }

    public async Task FunctionHandler(HealthCheckEvent healthCheckEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Lambda triggered. IntervalSeconds: {healthCheckEvent.IntervalSeconds}");

        var endpoints = await _endpointRepository.GetEndpointsByIntervalAsync(healthCheckEvent.IntervalSeconds);
        context.Logger.LogInformation($"Found {endpoints.Count()} endpoints to check");

        foreach (var endpoint in endpoints)
        {
            context.Logger.LogInformation($"Checking endpoint: {endpoint.Url}");
            var result = await _healthCheckService.RunHealthCheckAsync(endpoint.Id, endpoint.Url);
            result.UserId = endpoint.UserId;

            context.Logger.LogInformation($"Result: {result.Status} | StatusCode: {result.StatusCode} | Latency: {result.LatencyMs}ms");

            await _snsPublisher.PublishAlertAsync(new AlertNotificationDto
            {
                EndpointId = result.EndpointId,
                Result = result,
                SentAt = DateTime.UtcNow
            });
            context.Logger.LogInformation($"Alert published for endpoint: {endpoint.Id}");
        }
    }
}