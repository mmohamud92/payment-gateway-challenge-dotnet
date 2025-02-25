using System.Text.Json.Serialization;

namespace PaymentGateway.Application.DTOs;

public record BankPaymentRequestDto(
    [property: JsonPropertyName("card_number")]
    string CardNumber,
    [property: JsonPropertyName("expiry_date")]
    string ExpiryDate,
    [property: JsonPropertyName("currency")]
    string Currency,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("cvv")] string Cvv
);