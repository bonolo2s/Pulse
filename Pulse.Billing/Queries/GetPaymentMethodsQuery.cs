using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Queries;

public record GetPaymentMethodsQuery(Guid UserId) : IRequest<IEnumerable<PaymentMethod>>;