using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Pulse.Lambda.Interfaces;
using Pulse.Lambda.Models;
using Pulse.Shared.Interfaces;
using Pulse.Shared.DTOs;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Pulse.Lambda;

public class HealthCheckFunction
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ISnsPublisher _snsPublisher;

    public HealthCheckFunction(IHealthCheckService healthCheckService, ISnsPublisher snsPublisher)
    {
        _healthCheckService = healthCheckService;
        _snsPublisher = snsPublisher;
    }

    public async Task FunctionHandler(HealthCheckPayload payload, ILambdaContext context)
    {
        var result = await _healthCheckService.RunHealthCheckAsync(payload.EndpointId, payload.Url);

        await _snsPublisher.PublishAlertAsync(new AlertNotificationDto
        {
            EndpointId = result.EndpointId,
            Message = JsonSerializer.Serialize(result),
            SentAt = DateTime.UtcNow
        });
    }
}