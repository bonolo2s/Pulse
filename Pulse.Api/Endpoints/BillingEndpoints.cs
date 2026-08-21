using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.DTOs;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Paystack;
using Pulse.Billing.Payments.Paystack.DTOs;
using Pulse.Billing.Queries;
using Pulse.Shared.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

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

        group.MapPost("/checkout", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var email = user.FindFirstValue(ClaimTypes.Email)!;

            var result = await mediator.Send(new InitiateCheckoutCommand(userId, email));
            return Results.Ok(ApiResponse<InitializeTransactionResult>.Success(result, "Checkout initiated successfully."));
        })
        .Produces<ApiResponse<InitializeTransactionResult>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("InitiateCheckout")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/webhooks/payment", async (HttpRequest request, IMediator mediator, IConfiguration configuration, IBillingEventWriter eventWriter) =>
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            var remoteIp = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var signatureHeader = request.Headers["x-paystack-signature"].FirstOrDefault();
            var secretKey = configuration["Paystack:SecretKey"]!;

            var ipOk = PaystackWebhookValidator.IsIpWhitelisted(remoteIp);
            var signatureOk = PaystackWebhookValidator.IsSignatureValid(rawBody, signatureHeader, secretKey);

            if (!ipOk && !signatureOk)
            {
                await eventWriter.LogEventAsync(
                    eventType: BillingEventType.WebhookRejected,
                    source: BillingEventSource.Webhook,
                    paymentId: null,
                    userId: null,
                    paystackEventId: null,
                    payload: rawBody,
                    previousStatus: null,
                    newStatus: null);

                return Results.Unauthorized();
                //theres a silennt bug...on BE doesnt upfare but UI says success
            }


            var payload = JsonSerializer.Deserialize<PaystackWebhookPayload>(rawBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload?.Event == "charge.success")
            {
                await mediator.Send(new ProcessPaymentResultCommand(payload.Data.Reference, payload.Data.Status));
            }

            return Results.NoContent();
        })
        .WithName("SyncPaymentWebhook")
        .WithTags("Billing")
        .WithOpenApi();

        group.MapGet("/get-payment-methods/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var results = await mediator.Send(new GetPaymentMethodsQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<PaymentMethodResponse>>.Success(results.Select(pm => new PaymentMethodResponse
            {
                Id = pm.Id,
                Type = pm.Type,
                Brand = pm.Brand,
                Last4 = pm.Last4,
                ExpiryMonth = pm.ExpiryMonth,
                ExpiryYear = pm.ExpiryYear,
                BankName = pm.BankName,
                IsDefault = pm.IsDefault
            }), "Payment methods retrieved successfully."));
        })
        .Produces<ApiResponse<IEnumerable<PaymentMethodResponse>>>(200)
        .Produces<ApiResponse<object>>(401)
        .WithName("GetPaymentMethods")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/payment-methods/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeletePaymentMethodCommand(id));
            return Results.Ok(ApiResponse<object>.Success(null, "Payment method removed."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .WithName("DeletePaymentMethod")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/payment-methods/{id:guid}/set-default", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new SetDefaultPaymentMethodCommand(id));
            return Results.Ok(ApiResponse<object>.Success(null, "Default payment method updated."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .WithName("SetDefaultPaymentMethod")
        .WithTags("Billing")
        .WithOpenApi()
        .RequireAuthorization();
    }
}