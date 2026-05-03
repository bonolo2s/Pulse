using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class SyncPaymentWebhookHandler : IRequestHandler<SyncPaymentWebhookCommand>
{
    private readonly IBillingService _billingService;

    public SyncPaymentWebhookHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task Handle(SyncPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        await _billingService.SyncPaymentWebhookAsync(request.PaymentReference, request.Status);
    }
}