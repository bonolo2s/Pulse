using Microsoft.Extensions.Configuration;
using Pulse.Billing.Payments.Paystack.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Pulse.Billing.Payments.Paystack;

public class PaystackPaymentProvider : IPaymentProvider
{
    private readonly HttpClient _httpClient;

    public PaystackPaymentProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var secretKey = configuration["Paystack:SecretKey"];
        _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", secretKey);
    }

    public async Task<InitializeTransactionResult> InitializeTransaction(InitializeTransactionRequest request)
    {
        var payload = new
        {
            email = request.Email,
            amount = (int)(request.Amount * 100),
            currency = request.Currency,
            callback_url = request.CallbackUrl
        };

        var response = await _httpClient.PostAsJsonAsync("transaction/initialize", payload);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var data = json.GetProperty("data");

        return new InitializeTransactionResult(
            data.GetProperty("authorization_url").GetString()!,
            data.GetProperty("access_code").GetString()!,
            data.GetProperty("reference").GetString()!
        );
    }
}