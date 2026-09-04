using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;
namespace Pulse.Billing.Handlers;
public class SyncPaymentWebhookHandler : IRequestHandler<ProcessPaymentResultCommand>
{
    private readonly IBillingService _billingService;
    public SyncPaymentWebhookHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }
    public async Task Handle(ProcessPaymentResultCommand request, CancellationToken cancellationToken)
    {
        await _billingService.ProcessPaymentResultAsync(request.PaymentReference, request.EventId, request.Status, request.Channel, request.Email, request.Authorization);
    }
}