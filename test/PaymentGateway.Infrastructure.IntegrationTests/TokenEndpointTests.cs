using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json.Linq;

namespace PaymentGateway.Infrastructure.IntegrationTests;

[Trait("Category", "Integration")]
public class TokenEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PasswordGrant_WithValidCredentials_ReturnsAccessToken()
    {
        // Arrange
        FormUrlEncodedContent form = new([
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", "superuser"),
            new KeyValuePair<string, string>("password", "superpassword"),
            new KeyValuePair<string, string>("scope", "payment.read payment.write")
        ]);

        // Act
        HttpResponseMessage response = await _client.PostAsync("/connect/token", form);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string json = await response.Content.ReadAsStringAsync();
        JObject tokenResponse = JObject.Parse(json);
        Assert.NotNull(tokenResponse["access_token"]);
        Assert.True(tokenResponse["access_token"] is { Type: JTokenType.String },
            "Expected access_token to be a string.");
        Assert.NotNull(tokenResponse["expires_in"]);
        Assert.NotNull(tokenResponse["token_type"]);
    }

    [Fact]
    public async Task PasswordGrant_WithInvalidCredentials_ReturnsError()
    {
        FormUrlEncodedContent form = new([
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", "superuser"),
            new KeyValuePair<string, string>("password", "wr0nGpaSsWord"),
            new KeyValuePair<string, string>("scope", "payment.read payment.write")
        ]);

        // Act
        HttpResponseMessage response = await _client.PostAsync("/connect/token", form);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string json = await response.Content.ReadAsStringAsync();
        JObject errorResponse = JObject.Parse(json);
        Assert.NotNull(errorResponse["error"]);
        Assert.Equal("invalid_grant", (string)errorResponse["error"]!);
    }
}