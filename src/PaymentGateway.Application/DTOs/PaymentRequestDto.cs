namespace PaymentGateway.Application.DTOs;

public record PaymentRequestDto(
    string CardNumber,
    string ExpiryMonth,
    string ExpiryYear,
    string Currency,
    int Amount,
    string Cvv
);