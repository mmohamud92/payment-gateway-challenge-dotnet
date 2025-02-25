using System.Text.Json.Serialization;

namespace PaymentGateway.Application.DTOs;

public record PaymentResponseDto(
    Guid Id,
    string Status,
    [property: JsonPropertyName("last_four_digits")]
    string LastFourCardDigits,
    [property: JsonPropertyName("expiry_month")]
    int ExpiryMonth,
    [property: JsonPropertyName("expiry_year")]
    int ExpiryYear,
    string Currency,
    int Amount
);