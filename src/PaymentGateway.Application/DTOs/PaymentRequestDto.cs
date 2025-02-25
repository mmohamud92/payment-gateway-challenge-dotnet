namespace PaymentGateway.Application.DTOs;

public record PaymentRequestDto(
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string Cvv
);