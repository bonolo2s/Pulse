using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Payments.Interfaces;

public interface IPaymentProvider
{
    Task<InitializeTransactionResult> InitializeTransaction(InitializeTransactionRequest request);

    //Task<VerifyTransactionResult> VerifyTransaction(string reference);// fallback for Ghost webhooks that dont arrive

    //Task<CreateCustomerResult> CreateCustomer(CreateCustomerRequest request);

    //Task<CustomerResult> GetCustomer(string customerCode);

    //Task<CreateSubscriptionResult> CreateSubscription(CreateSubscriptionRequest request);

    //Task<SubscriptionResult> GetSubscription(string subscriptionCode);

    //Task<DisableSubscriptionResult> DisableSubscription(DisableSubscriptionRequest request);

    //Task<ChargeAuthorizationResult> ChargeAuthorization(ChargeAuthorizationRequest request);

    //Task<ListBanksResult> ListBanks();
}