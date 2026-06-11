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

    public async Task FunctionHandler(HealthCheckEvent healthCheckEvent, ILambdaContext context) //rules meteData
    {
        var endpoints = await _endpointRepository.GetEndpointsByIntervalAsync(healthCheckEvent.IntervalSeconds);

        foreach (var endpoint in endpoints)
        {
            var result = await _healthCheckService.RunHealthCheckAsync(endpoint.Id, endpoint.Url);
            await _snsPublisher.PublishAlertAsync(new AlertNotificationDto
            {
                EndpointId = result.EndpointId,
                Result = result,
                SentAt = DateTime.UtcNow
            });
        }
    }
}