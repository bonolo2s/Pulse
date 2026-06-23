using MediatR;
using System.Text.Json;
using Pulse.Shared.DTOs;
using Pulse.Observability.Commands;
using Pulse.Observability.Interfaces;
using Pulse.Observability.Entities;

namespace Pulse.Observability.Services;

public class RecordResultMessageProcessor : IRecordResultMessageProcessor
{
    private readonly IMediator _mediator;

    public RecordResultMessageProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProcessAsync(string messageBody, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<SnsEnvelope>(messageBody);
        var dto = JsonSerializer.Deserialize<AlertNotificationDto>(envelope!.Message);

        var result = new CheckResult
        {
            EndpointId = dto!.Result.EndpointId,
            Status = dto.Result.Status,
            StatusCode = dto.Result.StatusCode,
            LatencyMs = dto.Result.LatencyMs,
            SslIssuer = dto.Result.SslIssuer,
            SslExpiresAt = dto.Result.SslExpiresAt,
            SslDaysRemaining = dto.Result.SslDaysRemaining,
            ErrorMessage = dto.Result.ErrorMessage
        };

        await _mediator.Send(new RecordCheckResultCommand(result), cancellationToken);
    }
}