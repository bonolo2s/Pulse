using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.DTOs;
using Pulse.Monitoring.Queries;
using Pulse.Shared.Results;

namespace Pulse.Api.Endpoints;

public static class MonitoringEndpoints
{
    public static void MapMonitoringEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/monitoring");

        group.MapPost("/add-endpoint", async (AddEndpointRequest request, IMediator mediator) =>
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
            return Results.Created($"/api/monitoring/endpoints/{result.Id}", ApiResponse<EndpointResponse>.Success(new EndpointResponse
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
            }, "Endpoint added successfully."));
        })
        .Produces<ApiResponse<EndpointResponse>>(201)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("AddEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/edit-endpoint/{id:guid}", async (Guid id, EditEndpointRequest request, IMediator mediator) =>
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
            return Results.Ok(ApiResponse<EndpointResponse>.Success(new EndpointResponse
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
            }, "Endpoint updated successfully."));
        })
        .Produces<ApiResponse<EndpointResponse>>(200)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("EditEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/remove-endpoint/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new RemoveEndpointCommand(id));
            return Results.Ok(ApiResponse<object>.Success(null, "Endpoint removed successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("RemoveEndpoint")
        .WithTags("Monitoring")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPatch("/toggle-monitor/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new ToggleMonitorStatusCommand(id));
            return Results.Ok(ApiResponse<object>.Success(null, "Monitor status toggled successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("ToggleMonitorStatus")
        .WithTags("Monitoring")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-endpoints/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetEndpointsQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<EndpointResponse>>.Success(results.Select(e => new EndpointResponse
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
            }), "Endpoints retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<EndpointResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetEndpoints")
        .WithTags("Monitoring")
        .WithOpenApi()
        .RequireAuthorization();
    }
}