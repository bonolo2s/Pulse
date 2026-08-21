using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class DeletePaymentMethodHandler : IRequestHandler<DeletePaymentMethodCommand>
{
    private readonly IPaymentMethodService _paymentMethodService;

    public DeletePaymentMethodHandler(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        await _paymentMethodService.DeletePaymentMethodAsync(request.Id);
    }
}