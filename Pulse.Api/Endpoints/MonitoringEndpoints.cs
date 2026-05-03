using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.DTOs;
using Pulse.Monitoring.Queries;

namespace Pulse.Api.Endpoints;

public static class MonitoringEndpoints
{
    public static void MapMonitoringEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/monitoring");

        group.MapPost("/endpoints", async (AddEndpointRequest request, IMediator mediator) =>
        {
            var endpoint = new MonitoredEndpoint
            {
                UserId = request.UserId,
                Name = request.Name,
                Url = request.Url,
                Method = request.Method,
                IntervalSeconds = request.IntervalSeconds,
                TimeoutMs = request.TimeoutMs
            };

            var result = await mediator.Send(new AddEndpointCommand(endpoint));

            return Results.Created($"/api/monitoring/endpoints/{result.Id}", new EndpointResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Url = result.Url,
                Method = result.Method,
                IntervalSeconds = result.IntervalSeconds,
                TimeoutMs = result.TimeoutMs,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt
            });
        })
        .WithName("AddEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi();

        group.MapPut("/endpoints/{id:guid}", async (Guid id, EditEndpointRequest request, IMediator mediator) =>
        {
            var endpoint = new MonitoredEndpoint
            {
                Name = request.Name,
                Url = request.Url,
                Method = request.Method,
                IntervalSeconds = request.IntervalSeconds,
                TimeoutMs = request.TimeoutMs
            };

            var result = await mediator.Send(new EditEndpointCommand(id, endpoint));

            return Results.Ok(new EndpointResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Url = result.Url,
                Method = result.Method,
                IntervalSeconds = result.IntervalSeconds,
                TimeoutMs = result.TimeoutMs,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt
            });
        })
        .WithName("EditEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi();

        group.MapDelete("/endpoints/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new RemoveEndpointCommand(id));
            return Results.NoContent();
        })
        .WithName("RemoveEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi();

        group.MapPatch("/endpoints/{id:guid}/toggle", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new ToggleMonitorStatusCommand(id));
            return Results.NoContent();
        })
        .WithName("ToggleMonitorStatus")
        .WithTags("Monitoring")
        .WithOpenApi();

        group.MapGet("/endpoints/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetEndpointsQuery(userId));

            return Results.Ok(results.Select(e => new EndpointResponse
            {
                Id = e.Id,
                UserId = e.UserId,
                Name = e.Name,
                Url = e.Url,
                Method = e.Method,
                IntervalSeconds = e.IntervalSeconds,
                TimeoutMs = e.TimeoutMs,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            }));
        })
        .WithName("GetEndpoints")
        .WithTags("Monitoring")
        .WithOpenApi();
    }
}