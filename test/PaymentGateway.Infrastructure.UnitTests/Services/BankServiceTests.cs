using System.Net;
using System.Text.Json;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Infrastructure.Services;
using PaymentGateway.Infrastructure.UnitTests.Helpers;

namespace PaymentGateway.Infrastructure.UnitTests.Services;

[Trait("Category", "Unit")]
public class BankServiceTests
{
    [Fact]
    public async Task AuthorisePaymentAsync_ValidRequest_ReturnsExpectedResponse()
    {
        // Arrange: Create a sample BankPaymentRequestDto.
        BankPaymentRequestDto bankRequest = new(
            "4111111111111111",
            "04/2025",
            "GBP",
            1050,
            "123");

        // Create a fake bank response.
        BankPaymentResponseDto bankResponse = new(true, "ABC123");
        string responseJson = JsonSerializer.Serialize(bankResponse);

        FakeHttpMessageHandler handler = new(responseJson, HttpStatusCode.OK);
        HttpClient httpClient = new(handler) { BaseAddress = new Uri("http://localhost:8080/") };

        IBankService bankService = new BankService(httpClient);

        // Act
        BankPaymentResponseDto result = await bankService.AuthorisePaymentAsync(bankRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Authorised);
        Assert.Equal("ABC123", result.AuthorisationCode);
    }

    [Fact]
    public async Task AuthorisePaymentAsync_NonSuccessResponse_ThrowsException()
    {
        // Arrange: Create a sample BankPaymentRequestDto.
        BankPaymentRequestDto bankRequest = new(
            "4111111111111111",
            "04/2025",
            "GBP",
            1050,
            "123");

        const string errorResponse = "{\"error\":\"Bad Request\"}";
        FakeHttpMessageHandler handler = new(errorResponse, HttpStatusCode.BadRequest);
        HttpClient httpClient = new(handler) { BaseAddress = new Uri("http://localhost:8080/") };
        IBankService bankService = new BankService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await bankService.AuthorisePaymentAsync(bankRequest);
        });
    }
}