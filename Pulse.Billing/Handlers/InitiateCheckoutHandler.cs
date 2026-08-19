using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulse.Billing.Commands;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments;
using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Handlers;

public class InitiateCheckoutHandler : IRequestHandler<InitiateCheckoutCommand, InitializeTransactionResult>
{
    private readonly IPaymentProvider _paymentProvider;
    private readonly IBillingService _billingService;
    private readonly BillingDbContext _context;
    private readonly IConfiguration _configuration;

    public InitiateCheckoutHandler(IPaymentProvider paymentProvider, IBillingService billingService, BillingDbContext context, IConfiguration configuration)
    {
        _paymentProvider = paymentProvider;
        _billingService = billingService;
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
        const string currency = "ZAR";

        var paystackRequest = new InitializeTransactionRequest(
            request.Email,
            amount,
            currency,
            callbackUrl
        );

        var result = await _paymentProvider.InitializeTransaction(paystackRequest);

        var invoice = await _billingService.CreatePendingInvoiceAsync(request.UserId, subscription.Id, amount, currency);
        await _billingService.CreatePendingPaymentAsync(request.UserId, invoice.Id, amount, result.Reference);

        return result;
    }
}