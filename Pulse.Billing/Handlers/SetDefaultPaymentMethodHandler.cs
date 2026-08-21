using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class SetDefaultPaymentMethodHandler : IRequestHandler<SetDefaultPaymentMethodCommand>
{
    private readonly IPaymentMethodService _paymentMethodService;

    public SetDefaultPaymentMethodHandler(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    public async Task Handle(SetDefaultPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        await _paymentMethodService.SetDefaultPaymentMethodAsync(request.Id);
    }
}