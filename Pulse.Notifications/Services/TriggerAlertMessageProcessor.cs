using MediatR;
using System.Text.Json;
using Pulse.Shared.DTOs;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Services;

public class TriggerAlertMessageProcessor : ITriggerAlertMessageProcessor
{
    private readonly IMediator _mediator;

    public TriggerAlertMessageProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProcessAsync(string messageBody, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<SnsEnvelope>(messageBody);
        var dto = JsonSerializer.Deserialize<AlertNotificationDto>(envelope!.Message);
        await _mediator.Send(new TriggerAlertCommand(dto!.Result), cancellationToken);
    }
}