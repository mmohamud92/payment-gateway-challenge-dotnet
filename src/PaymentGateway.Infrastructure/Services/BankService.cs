using System.Net;
using System.Text;
using System.Text.Json;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Infrastructure.Services;

public class BankService(HttpClient httpClient) : IBankService
{
    public async Task<BankPaymentResponseDto> AuthorisePaymentAsync(BankPaymentRequestDto bankPaymentRequest)
    {
        string json = JsonSerializer.Serialize(bankPaymentRequest);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync("payments", content);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        BankPaymentResponseDto? bankResponse = JsonSerializer.Deserialize<BankPaymentResponseDto>(responseJson);

        if (bankResponse == null)
        {
            throw new Exception("Unable to deserialise the bank response.");
        }

        return bankResponse;
    }
}