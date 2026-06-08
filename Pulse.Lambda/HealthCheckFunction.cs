using Amazon.Lambda.Core;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Pulse.Lambda.Interfaces;
using Pulse.Shared.Interfaces;
using Pulse.Shared.DTOs;
using Pulse.Lambda.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Pulse.Lambda;

public class HealthCheckFunction
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ISnsPublisher _snsPublisher;
    private readonly IEndpointRepository _endpointRepository;

    public HealthCheckFunction(
        IHealthCheckService healthCheckService,
        ISnsPublisher snsPublisher,
        IEndpointRepository endpointRepository)
    {
        _healthCheckService = healthCheckService;
        _snsPublisher = snsPublisher;
        _endpointRepository = endpointRepository;
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