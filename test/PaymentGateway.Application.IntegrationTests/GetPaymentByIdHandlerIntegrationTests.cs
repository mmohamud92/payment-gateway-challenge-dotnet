using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Handlers;
using PaymentGateway.Application.MappingProfiles;
using PaymentGateway.Application.Queries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Application.IntegrationTests;

[Trait("Category", "Integration")]
public class GetPaymentByIdHandlerIntegrationTests
{
    private readonly IMapper _mapper;
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByIdHandlerIntegrationTests()
    {
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddProfile<PaymentMappingProfile>();
        });
        _mapper = config.CreateMapper();
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        _paymentRepository = new PaymentRepository(memoryCache);
    }

    [Fact]
    public async Task GetPaymentByIdHandler_ReturnsPaymentResponseDto_WhenPaymentExists()
    {
        // Arrange:
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        GetPaymentByIdQuery query = new(payment.Id);
        GetPaymentByIdHandler handler = new(_paymentRepository, _mapper);

        // Act:
        PaymentResponseDto response = await handler.Handle(query, CancellationToken.None);

        // Assert:
        Assert.Equal(payment.Id, response.Id);
        Assert.Equal(payment.Status.ToString(), response.Status);
        Assert.Equal(payment.LastFourDigits, response.LastFourCardDigits);
        Assert.Equal(payment.CardDetails.ExpiryDate.Month.ToString(), response.ExpiryMonth);
        Assert.Equal(payment.CardDetails.ExpiryDate.Year.ToString(), response.ExpiryYear);
        Assert.Equal(payment.Denomination.Currency.ToString(), response.Currency);
        Assert.Equal(payment.Denomination.Amount, response.Amount);
    }

    [Fact]
    public async Task GetPaymentByIdHandler_ThrowsPaymentNotFoundException_WhenPaymentDoesNotExist()
    {
        // Arrange
        Guid nonExistentPaymentId = Guid.NewGuid();
        GetPaymentByIdQuery query = new(nonExistentPaymentId);
        GetPaymentByIdHandler handler = new(_paymentRepository, _mapper);

        // Act & Assert:
        PaymentNotFoundException ex = await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(query, CancellationToken.None)
        );
        Assert.Contains(nonExistentPaymentId.ToString(), ex.Message);
    }

    private static Payment CreateValidPayment()
    {
        Guid merchantId = Guid.NewGuid();
        const string validCardNumber = "4111111111111111";
        const int amount = 10000;
        const string currency = "GBP";
        const string cvv = "123";
        const string expiryMonth = "12";
        string expiryYear = $"{DateTime.UtcNow.Year + 1}";

        return new Payment(merchantId, validCardNumber, expiryMonth, expiryYear, amount, currency, cvv);
    }
}