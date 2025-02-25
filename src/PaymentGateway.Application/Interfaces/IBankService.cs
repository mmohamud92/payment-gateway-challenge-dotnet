using PaymentGateway.Application.DTOs;

namespace PaymentGateway.Application.Interfaces;

public interface IBankService
{
    Task<BankPaymentResponseDto> AuthorisePaymentAsync(BankPaymentRequestDto bankPaymentRequest);
}