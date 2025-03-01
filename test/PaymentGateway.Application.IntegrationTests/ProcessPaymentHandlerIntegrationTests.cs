using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Handlers;
using PaymentGateway.Application.IntegrationTests.Helpers;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.MappingProfiles;
using PaymentGateway.Application.Queries;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Application.IntegrationTests;

[Trait("Category", "Integration")]
public class ProcessPaymentHandlerIntegrationTests
{
    private readonly IMapper _mapper;
    private readonly IBankService _bankService;
    private readonly IPaymentRepository _paymentRepository;

    public ProcessPaymentHandlerIntegrationTests()
    {
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddProfile<PaymentMappingProfile>();
        });
        _mapper = config.CreateMapper();
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        _bankService = new FakeBankService();
        _paymentRepository = new PaymentRepository(memoryCache);
    }

    [Fact]
    public async Task ProcessAndRetrievePayment_IntegrationTest()
    {
        // Arrange:
        Guid merchantId = Guid.NewGuid();
        PaymentRequestDto paymentRequest = new(
            "4111111111111111",
            "12",
            $"{DateTime.UtcNow.Year + 1}",
            "GBP",
            10000,
            "123"
        );

        ProcessPaymentCommand processCommand = new(paymentRequest, merchantId);
        ProcessPaymentHandler processHandler = new(_mapper, _bankService, _paymentRepository);

        // Act:
        PaymentResponseDto processedResponse = await processHandler.Handle(processCommand, CancellationToken.None);
        // Assert:
        Assert.Equal(PaymentStatus.Authorised.ToString(), processedResponse.Status);
        Assert.NotEqual(Guid.Empty, processedResponse.Id);
    }

    [Fact]
    public async Task ProcessPaymentHandler_InvalidPayment_ThrowsPaymentValidationException()
    {
        // Arrange
        Guid merchantId = Guid.NewGuid();
        PaymentRequestDto invalidPaymentRequest = new(
            "4111111111111111",
            "12",
            $"{DateTime.UtcNow.Year + 1}",
            "GBP",
            -100,
            "123"
        );
        ProcessPaymentCommand command = new(invalidPaymentRequest, merchantId);
        ProcessPaymentHandler handler = new(_mapper, _bankService, _paymentRepository);

        // Act & Assert
        PaymentValidationException ex = await Assert.ThrowsAsync<PaymentValidationException>(
            () => handler.Handle(command, CancellationToken.None)
        );
        Assert.NotNull(ex.Message);
        Assert.Contains("Amount cannot be negative", ex.Message);
    }
}