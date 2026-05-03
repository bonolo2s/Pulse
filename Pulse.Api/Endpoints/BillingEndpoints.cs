using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.DTOs;
using Pulse.Billing.Entities;
using Pulse.Billing.Queries;

namespace Pulse.Api.Endpoints;

public static class BillingEndpoints
{
    public static void MapBillingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/billing");

        group.MapPost("/subscriptions/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var subscription = new Subscription { UserId = userId };
            var result = await mediator.Send(new CreateSubscriptionCommand(subscription));

            return Results.Created($"/api/billing/subscriptions/{result.Id}", new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            });
        })
        .WithName("CreateSubscription")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapPut("/subscriptions/{userId:guid}/upgrade", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpgradeToProCommand(userId));

            return Results.Ok(new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            });
        })
        .WithName("UpgradeToPro")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapPut("/subscriptions/{userId:guid}/cancel", async (Guid userId, IMediator mediator) =>
        {
            await mediator.Send(new CancelSubscriptionCommand(userId));
            return Results.NoContent();
        })
        .WithName("CancelSubscription")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapGet("/subscriptions/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSubscriptionQuery(userId));

            return Results.Ok(new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            });
        })
        .WithName("GetSubscription")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapGet("/invoices/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetBillingHistoryQuery(userId));

            return Results.Ok(results.Select(i => new InvoiceResponse
            {
                Id = i.Id,
                UserId = i.UserId,
                SubscriptionId = i.SubscriptionId,
                Amount = i.Amount,
                Currency = i.Currency,
                Status = i.Status,
                PaymentReference = i.PaymentReference,
                IssuedAt = i.IssuedAt,
                PaidAt = i.PaidAt
            }));
        })
        .WithName("GetBillingHistory")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapPost("/webhooks/payment", async (SyncPaymentWebhookRequest request, IMediator mediator) => //webhook tirggered
        {
            await mediator.Send(new SyncPaymentWebhookCommand(request.PaymentReference, request.Status));
            return Results.NoContent();
        })
        .WithName("SyncPaymentWebhook")
        .WithTags("Billing")
        .WithOpenApi();
    }
}