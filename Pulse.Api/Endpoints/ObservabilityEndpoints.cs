using MediatR;
using Pulse.Observability.Commands;
using Pulse.Observability.DTOs;
using Pulse.Observability.Entities;
using Pulse.Observability.Queries;
using Pulse.Shared.DTOs;
using Pulse.Shared.Results;

namespace Pulse.Api.Endpoints;

public static class ObservabilityEndpoints
{
    public static void MapObservabilityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/observability");

        group.MapPost("/record-result", async (AlertNotificationDto dto, IMediator mediator) =>
        {
            var result = new CheckResult
            {
                EndpointId = dto.Result.EndpointId,
                Status = dto.Result.Status,
                StatusCode = dto.Result.StatusCode,
                LatencyMs = dto.Result.LatencyMs,
                SslIssuer = dto.Result.SslIssuer,
                SslExpiresAt = dto.Result.SslExpiresAt,
                SslDaysRemaining = dto.Result.SslDaysRemaining,
                ErrorMessage = dto.Result.ErrorMessage
            };
            await mediator.Send(new RecordCheckResultCommand(result));
            return Results.NoContent();
        })
        .WithName("RecordCheckResult") // fired by SNS — no auth
        .WithTags("Observability")
        .WithOpenApi();

        group.MapGet("/get-uptime/{endpointId:guid}", async (Guid endpointId, int days, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetUptimeHistoryQuery(endpointId, days));
            return Results.Ok(ApiResponse<IEnumerable<CheckResultResponse>>.Success(results.Select(e => new CheckResultResponse
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
            }), "Uptime history retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<CheckResultResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetUptimeHistory")
        .WithTags("Observability")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-latency/{endpointId:guid}", async (Guid endpointId, int days, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetLatencyTrendsQuery(endpointId, days));
            return Results.Ok(ApiResponse<IEnumerable<CheckResultResponse>>.Success(results.Select(e => new CheckResultResponse
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
            }), "Latency trends retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<CheckResultResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetLatencyTrends")
        .WithTags("Observability")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-ssl/{endpointId:guid}", async (Guid endpointId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSslExpiryStatusQuery(endpointId));
            if (result is null) return Results.NotFound(ApiResponse<object>.Failure("SSL data not found for this endpoint."));
            return Results.Ok(ApiResponse<CheckResultResponse>.Success(new CheckResultResponse
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
            }, "SSL expiry status retrieved successfully."));
        })
        .Produces<ApiResponse<CheckResultResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetSslExpiryStatus")
        .WithTags("Observability")
        .WithOpenApi()
        .RequireAuthorization();
    }
}