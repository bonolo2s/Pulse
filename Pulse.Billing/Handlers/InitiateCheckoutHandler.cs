using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulse.Billing.Commands;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Payments;
using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Handlers;

public class InitiateCheckoutHandler : IRequestHandler<InitiateCheckoutCommand, InitializeTransactionResult>
{
    private readonly IPaymentProvider _paymentProvider;
    private readonly BillingDbContext _context;
    private readonly IConfiguration _configuration;

    public InitiateCheckoutHandler(IPaymentProvider paymentProvider, BillingDbContext context, IConfiguration configuration)
    {
        _paymentProvider = paymentProvider;
        _context = context;
        _configuration = configuration;
    }

    public async Task<InitializeTransactionResult> Handle(InitiateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException($"Subscription for user {request.UserId} not found.");

        var amount = _configuration.GetValue<decimal>("Paystack:Plans:Pro");
        var callbackUrl = _configuration["Paystack:CallbackUrl"]!;

        var paystackRequest = new InitializeTransactionRequest(
            request.Email,
            amount,
            "ZAR",
            callbackUrl
        );

        var result = await _paymentProvider.InitializeTransaction(paystackRequest);

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SubscriptionId = subscription.Id,
            Amount = amount,
            Currency = "ZAR",
            Status = InvoiceStatus.Pending,
            IssuedAt = DateTime.UtcNow
        };
        _context.Invoices.Add(invoice);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            InvoiceId = invoice.Id,
            Amount = amount,
            Status = PaymentStatus.Pending,
            Method = PaymentMethodType.Card,
            Provider = "Paystack",
            ProviderReference = result.Reference,
            CreatedAt = DateTime.UtcNow
        };
        _context.Payments.Add(payment);

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}