using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pulse.Notifications.Commands;
using Pulse.Notifications.DTOs;
using Pulse.Notifications.Entities;
using Pulse.Notifications.Queries;
using Pulse.Shared.DTOs;
using Pulse.Shared.Results;
using System.Text.Json;

namespace Pulse.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications");

        //group.MapPost("/sns", async (HttpRequest request, HttpClient httpClient) =>
        //{
        //    Console.WriteLine("Got SNS payload");

        //    request.Body.Position = 0;

        //    using var reader = new StreamReader(request.Body, leaveOpen: true);
        //    var body = await reader.ReadToEndAsync();

        //    Console.WriteLine($"Raw body: {body}");

        //    if (string.IsNullOrWhiteSpace(body))
        //        return Results.BadRequest(ApiResponse<object>.Failure("Empty request body."));

        //    var payload = JsonSerializer.Deserialize<JsonElement>(body);

        //    string type = payload.GetProperty("Type").GetString();

        //    if (type == "SubscriptionConfirmation")
        //    {
        //        string subscribeUrl = payload.GetProperty("SubscribeURL").GetString();
        //        await httpClient.GetAsync(subscribeUrl);
        //        return Results.Ok(ApiResponse<object>.Success(null, "Subscription confirmed."));
        //    }
        //    else if (type == "Notification")
        //    {
        //        string message = payload.GetProperty("Message").GetString();
        //        Console.WriteLine($"Received SNS message: {message}");
        //        return Results.Ok(ApiResponse<object>.Success(null, "Notification received."));
        //    }

        //    return Results.BadRequest(ApiResponse<object>.Failure("Unsupported SNS message type."));
        //})
        //.Produces<ApiResponse<object>>(200)
        //.Produces<ApiResponse<object>>(400)
        //.WithName("SnsHandler")
        //.WithTags("Notifications")
        //.WithOpenApi();



        group.MapPost("/trigger-alert", async (SnsEnvelope envelope, IMediator mediator, [FromServices] HttpClient httpClient) =>
        {
            Console.WriteLine("Got SNS payload");

            if (envelope.Type == "SubscriptionConfirmation")
            {
                Console.WriteLine("Triggering the handshake route.");
                await httpClient.GetAsync(envelope.SubscribeURL);
                return Results.Ok(ApiResponse<object>.Success(null, "Subscription confirmed."));
            }

            var dto = JsonSerializer.Deserialize<AlertNotificationDto>(envelope.Message);
            await mediator.Send(new TriggerAlertCommand(dto!.Result));//
            return Results.NoContent();
        })
        .WithName("TriggerAlert") // fired by Lambda/SNS
        .WithTags("Notifications")
        .WithOpenApi();

        group.MapPost("/resolve-alert/{endpointId:guid}", async (Guid endpointId, IMediator mediator) =>
        {
            await mediator.Send(new ResolveAlertCommand(endpointId));
            return Results.NoContent();
        })
        .WithName("ResolveAlert") // fired by Lambda/S
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