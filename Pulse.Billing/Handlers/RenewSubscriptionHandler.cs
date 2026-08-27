using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;
using Pulse.Billing.Payments.Paystack.DTOs;
namespace Pulse.Billing.Handlers;
public class RenewSubscriptionHandler : IRequestHandler<RenewSubscriptionCommand>
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly IInvoiceService _invoiceService;
    private readonly IPaymentProvider _paymentProvider;
    private readonly BillingDbContext _context;

    public RenewSubscriptionHandler(
        ISubscriptionService subscriptionService,
        IPaymentMethodService paymentMethodService,
        IInvoiceService invoiceService,
        IPaymentProvider paymentProvider,
        BillingDbContext context)
    {
        _subscriptionService = subscriptionService;
        _paymentMethodService = paymentMethodService;
        _invoiceService = invoiceService;
        _paymentProvider = paymentProvider;
        _context = context;
    }

    public async Task Handle(RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionService.GetSubscriptionForRenewalAsync(request.SubscriptionId);

        var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync(subscription.UserId);
        var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.IsDefault)
            ?? throw new InvalidOperationException($"No default payment method for user {subscription.UserId}.");

        var result = await _paymentProvider.ChargeAuthorization(new ChargeAuthorizationRequest(
            Email: request.Email,
            Amount: subscription.MonthlyPrice,
            AuthorizationCode: paymentMethod.AuthorizationCode
        ));

        var invoice = await _invoiceService.CreatePendingInvoiceAsync(subscription.UserId, subscription.Id, subscription.MonthlyPrice, "ZAR");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = subscription.UserId,
            InvoiceId = invoice.Id,
            Amount = subscription.MonthlyPrice,
            Status = PaymentStatus.Pending,
            Provider = "Paystack",
            ProviderReference = result.Reference,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }
}