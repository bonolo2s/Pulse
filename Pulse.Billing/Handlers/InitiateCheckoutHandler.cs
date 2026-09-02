using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulse.Billing.Commands;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;
using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Handlers;

public class InitiateCheckoutHandler : IRequestHandler<InitiateCheckoutCommand, InitializeTransactionResult>
{
    private readonly IPaymentProvider _paymentProvider;
    private readonly IBillingService _billingService;
    private readonly IInvoiceService _invoiceService;
    private readonly IBillingEventWriter _eventWriter;
    private readonly BillingDbContext _context;
    private readonly IConfiguration _configuration;
    public InitiateCheckoutHandler(
        IPaymentProvider paymentProvider,
        IBillingService billingService,
        IInvoiceService invoiceService,
        IBillingEventWriter eventWriter,
        BillingDbContext context,
        IConfiguration configuration)
    {
        _paymentProvider = paymentProvider;
        _billingService = billingService;
        _invoiceService = invoiceService;
        _eventWriter = eventWriter;
        _context = context;
        _configuration = configuration;
    }

    public async Task<InitializeTransactionResult> Handle(InitiateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException($"Subscription for user {request.UserId} not found.");

        var amount = _configuration.GetValue<decimal>("Paystack:Plans:Pro");
        var planCode = _configuration["Paystack:Plans:ProCode"]!;
        var callbackUrl = _configuration["Paystack:CallbackUrl"]!;
        const string currency = "ZAR";

        var paystackRequest = new InitializeTransactionRequest(
            request.Email,
            amount,
            currency,
            callbackUrl,
            planCode
        );

        var result = await _paymentProvider.InitializeTransaction(paystackRequest);

        await _eventWriter.LogEventAsync(
            eventType: BillingEventType.PaymentInitiated,
            source: BillingEventSource.Client,
            paymentId: null,
            userId: request.UserId,
            paystackEventId: result.Reference,
            payload: null,
            previousStatus: null,
            newStatus: null);

        return result;
    }
}