using System.Text.Json.Serialization;

namespace PaymentGateway.Application.DTOs;

public record BankPaymentResponseDto(
    [property: JsonPropertyName("authorized")]
    bool Authorised,
    [property: JsonPropertyName("authorization_code")]
    string AuthorisationCode
);