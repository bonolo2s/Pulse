using MediatR;
using Pulse.Observability.Commands;
using Pulse.Observability.DTOs;
using Pulse.Observability.Entities;
using Pulse.Observability.Queries;

namespace Pulse.Api.Endpoints;

public static class ObservabilityEndpoints
{
    public static void MapObservabilityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/observability");

        group.MapPost("/results", async (RecordCheckResultRequest request, IMediator mediator) =>
        {
            var result = new CheckResult
            {
                EndpointId = request.EndpointId,
                Status = request.Status,
                StatusCode = request.StatusCode,
                LatencyMs = request.LatencyMs,
                SslIssuer = request.SslIssuer,
                SslExpiresAt = request.SslExpiresAt,
                SslDaysRemaining = request.SslDaysRemaining,
                ErrorMessage = request.ErrorMessage
            };

            await mediator.Send(new RecordCheckResultCommand(result));
            return Results.NoContent();
        })
        .WithName("RecordCheckResult")
        .WithTags("Observability")
        .WithOpenApi();

        group.MapGet("/results/{endpointId:guid}/uptime", async (Guid endpointId, int days, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetUptimeHistoryQuery(endpointId, days));

            return Results.Ok(results.Select(e => new CheckResultResponse
            {
                Id = e.Id,
                EndpointId = e.EndpointId,
                Status = e.Status,
                StatusCode = e.StatusCode,
                LatencyMs = e.LatencyMs,
                SslIssuer = e.SslIssuer,
                SslExpiresAt = e.SslExpiresAt,
                SslDaysRemaining = e.SslDaysRemaining,
                ErrorMessage = e.ErrorMessage,
                CheckedAt = e.CheckedAt
            }));
        })
        .WithName("GetUptimeHistory")
        .WithTags("Observability")
        .WithOpenApi();

        group.MapGet("/results/{endpointId:guid}/latency", async (Guid endpointId, int days, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetLatencyTrendsQuery(endpointId, days));

            return Results.Ok(results.Select(e => new CheckResultResponse
            {
                Id = e.Id,
                EndpointId = e.EndpointId,
                Status = e.Status,
                StatusCode = e.StatusCode,
                LatencyMs = e.LatencyMs,
                SslIssuer = e.SslIssuer,
                SslExpiresAt = e.SslExpiresAt,
                SslDaysRemaining = e.SslDaysRemaining,
                ErrorMessage = e.ErrorMessage,
                CheckedAt = e.CheckedAt
            }));
        })
        .WithName("GetLatencyTrends")
        .WithTags("Observability")
        .WithOpenApi();

        group.MapGet("/results/{endpointId:guid}/ssl", async (Guid endpointId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSslExpiryStatusQuery(endpointId));

            if (result is null) return Results.NotFound();

            return Results.Ok(new CheckResultResponse
            {
                Id = result.Id,
                EndpointId = result.EndpointId,
                Status = result.Status,
                StatusCode = result.StatusCode,
                LatencyMs = result.LatencyMs,
                SslIssuer = result.SslIssuer,
                SslExpiresAt = result.SslExpiresAt,
                SslDaysRemaining = result.SslDaysRemaining,
                ErrorMessage = result.ErrorMessage,
                CheckedAt = result.CheckedAt
            });
        })
        .WithName("GetSslExpiryStatus")
        .WithTags("Observability")
        .WithOpenApi();
    }
}