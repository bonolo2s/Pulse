using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.DTOs;
using Pulse.Notifications.Entities;
using Pulse.Notifications.Queries;
using Pulse.Shared.DTOs;
using Pulse.Shared.Results;

namespace Pulse.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications");

        group.MapPost("/trigger-alert", async (AlertNotificationDto dto, IMediator mediator) =>
        {
            await mediator.Send(new TriggerAlertCommand(dto.Result));
            return Results.NoContent();
        })
        .WithName("TriggerAlert") // fired by lambda when  down
        .WithTags("Notifications")
        .WithOpenApi();

        group.MapPost("/resolve-alert/{endpointId:guid}", async (Guid endpointId, IMediator mediator) =>
        {
            await mediator.Send(new ResolveAlertCommand(endpointId));
            return Results.NoContent();
        })
        .WithName("ResolveAlert") // fired by lambda when  up/auto healed
        .WithTags("Notifications")
        .WithOpenApi();

        group.MapPost("/manage-rules", async (ManageAlertRulesRequest request, IMediator mediator) =>
        {
            var rule = new AlertRule
            {
                Id = request.AlertRuleId,
                UserId = request.UserId,
                Channel = request.Channel,
                Destination = request.Destination,
                IsActive = request.IsActive
            };
            await mediator.Send(new ManageAlertRulesCommand(rule));
            return Results.Ok(ApiResponse<object>.Success(null, "Alert rule saved successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("ManageAlertRules")
        .WithTags("Notifications")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-rules/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetAlertSettingsQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<AlertRuleResponse>>.Success(results.Select(r => new AlertRuleResponse
            {
                Id = r.Id,
                UserId = r.UserId,
                Channel = r.Channel,
                Destination = r.Destination,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt
            }), "Alert rules retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<AlertRuleResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetAlertSettings")
        .WithTags("Notifications")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-logs/{endpointId:guid}", async (Guid endpointId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetAlertLogsQuery(endpointId));
            return Results.Ok(ApiResponse<IEnumerable<AlertLogResponse>>.Success(results.Select(l => new AlertLogResponse
            {
                Id = l.Id,
                AlertRuleId = l.AlertRuleId,
                EndpointId = l.EndpointId,
                Channel = l.Channel,
                Type = l.Type,
                Message = l.Message,
                Delivered = l.Delivered,
                IsAcknowledged = l.IsAcknowledged,
                AcknowledgedAt = l.AcknowledgedAt,
                SentAt = l.SentAt
            }), "Alert logs retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<AlertLogResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetAlertLogs")
        .WithTags("Notifications")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPatch("/acknowledge-alert/{alertLogId:guid}", async (Guid alertLogId, IMediator mediator) =>
        {
            await mediator.Send(new AcknowledgeAlertCommand(alertLogId));
            return Results.Ok(ApiResponse<object>.Success(null, "Alert acknowledged successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("AcknowledgeAlert")
        .WithTags("Notifications")
        .WithOpenApi()
        .RequireAuthorization();
    }
}