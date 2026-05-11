using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.DTOs;
using Pulse.Billing.Entities;
using Pulse.Billing.Queries;
using Pulse.Shared.Results;

namespace Pulse.Api.Endpoints;

public static class BillingEndpoints
{
    public static void MapBillingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/billing");

        group.MapPost("/create-subscription/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var subscription = new Subscription { UserId = userId };
            var result = await mediator.Send(new CreateSubscriptionCommand(subscription));
            return Results.Created($"/api/billing/subscriptions/{result.Id}", ApiResponse<SubscriptionResponse>.Success(new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            }, "Subscription created successfully."));
        })
        .Produces<ApiResponse<SubscriptionResponse>>(201)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("CreateSubscription")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/upgrade-to-pro/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpgradeToProCommand(userId));
            return Results.Ok(ApiResponse<SubscriptionResponse>.Success(new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            }, "Subscription upgraded to Pro successfully."));
        })
        .Produces<ApiResponse<SubscriptionResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("UpgradeToPro")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/cancel-subscription/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            await mediator.Send(new CancelSubscriptionCommand(userId));
            return Results.Ok(ApiResponse<object>.Success(null, "Subscription cancelled successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("CancelSubscription")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-subscription/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSubscriptionQuery(userId));
            return Results.Ok(ApiResponse<SubscriptionResponse>.Success(new SubscriptionResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Plan = result.Plan,
                EndpointLimit = result.EndpointLimit,
                StartedAt = result.StartedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive
            }, "Subscription retrieved successfully."));
        })
        .Produces<ApiResponse<SubscriptionResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetSubscription")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/get-invoices/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetBillingHistoryQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<InvoiceResponse>>.Success(results.Select(i => new InvoiceResponse
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
            }), "Billing history retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<InvoiceResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetBillingHistory")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/webhooks/payment", async (SyncPaymentWebhookRequest request, IMediator mediator) => //webhook triggered
        {
            await mediator.Send(new SyncPaymentWebhookCommand(request.PaymentReference, request.Status));
            return Results.NoContent();
        })
        .WithName("SyncPaymentWebhook")
        .WithTags("Billing")
        .WithOpenApi();
    }
}