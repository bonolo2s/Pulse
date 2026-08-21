using MediatR;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetPaymentMethodsHandler : IRequestHandler<GetPaymentMethodsQuery, IEnumerable<PaymentMethod>>
{
    private readonly IPaymentMethodService _paymentMethodService;

    public GetPaymentMethodsHandler(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    public async Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        return await _paymentMethodService.GetPaymentMethodsAsync(request.UserId);
    }
}