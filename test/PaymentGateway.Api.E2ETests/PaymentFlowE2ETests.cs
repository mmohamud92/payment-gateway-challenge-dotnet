using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using PaymentGateway.Api.E2ETests.Fixtures;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Infrastructure;

namespace PaymentGateway.Api.E2ETests;

[Trait("Category", "E2E")]
public class PaymentFlowE2ETests(EndToEndTestFixture fixture) : IClassFixture<EndToEndTestFixture>
{
    private readonly HttpClient _client = fixture.Client!;

    [Fact(DisplayName = "Full Payment E2E Flow: Token -> POST Payment -> GET Payment")]
    public async Task PaymentFullFlow_ReturnsExpectedPayment()
    {
        string? writeToken = await GetAccessTokenAsync(Constants.ApiScopes.PaymentWriteScope);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", writeToken);

        var paymentRequest = new
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = DateTime.UtcNow.Month,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "GBP",
            Amount = 10000,
            Cvv = "123"
        };

        StringContent jsonContent = new(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
        HttpResponseMessage postResponse = await _client.PostAsync("/api/payments", jsonContent);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        string postResponseJson = await postResponse.Content.ReadAsStringAsync();
        PaymentResponseDto? createdPayment = JsonSerializer.Deserialize<PaymentResponseDto>(
            postResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(createdPayment);

        string? readToken = await GetAccessTokenAsync(Constants.ApiScopes.PaymentReadScope);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", readToken);

        HttpResponseMessage getResponse = await _client.GetAsync($"/api/payments/{createdPayment.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        string getResponseJson = await getResponse.Content.ReadAsStringAsync();
        PaymentResponseDto? retrievedPayment = JsonSerializer.Deserialize<PaymentResponseDto>(
            getResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(retrievedPayment);
        Assert.Equal(createdPayment.Id, retrievedPayment.Id);
    }

    private async Task<string?> GetAccessTokenAsync(string scope)
    {
        const string tokenEndpoint = "/connect/token";

        Dictionary<string, string> parameters = new()
        {
            { "grant_type", "password" },
            { "username", "superuser" },
            { "password", "superpassword" },
            { "scope", scope }
        };

        using FormUrlEncodedContent content = new(parameters);
        HttpResponseMessage response = await _client.PostAsync(tokenEndpoint, content);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        string? token = doc.RootElement.GetProperty("access_token").GetString();
        return token;
    }
}