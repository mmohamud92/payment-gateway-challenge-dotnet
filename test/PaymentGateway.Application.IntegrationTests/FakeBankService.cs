using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;

namespace PaymentGateway.Application.IntegrationTests;

public class FakeBankService : IBankService
{
    public Task<BankPaymentResponseDto> AuthorisePaymentAsync(BankPaymentRequestDto bankPaymentRequest)
    {
        BankPaymentResponseDto response = new(true, "TEST_AUTH_CODE");
        return Task.FromResult(response);
    }
}