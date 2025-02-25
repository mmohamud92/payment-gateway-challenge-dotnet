using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Handlers;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.MappingProfiles;
using PaymentGateway.Application.Queries;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Application.IntegrationTests;

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

    [Fact(DisplayName = "ProcessPaymentHandler and GetPaymentByIdHandler Integration Test")]
    public async Task ProcessAndRetrievePayment_IntegrationTest()
    {
        // Arrange:
        Guid merchantId = Guid.NewGuid();
        PaymentRequestDto paymentRequest = new(
            "4111111111111111",
            12,
            DateTime.UtcNow.Year + 1,
            "GBP",
            10000,
            "123"
        );

        ProcessPaymentCommand processCommand = new(paymentRequest, merchantId);

        // Create handler instances.
        ProcessPaymentHandler processHandler = new(_mapper, _bankService, _paymentRepository);
        GetPaymentByIdHandler getHandler = new(_paymentRepository, _mapper);

        // Act:
        PaymentResponseDto processedResponse = await processHandler.Handle(processCommand, CancellationToken.None);

        Assert.Equal(PaymentStatus.Authorised.ToString(), processedResponse.Status);
        Assert.NotEqual(Guid.Empty, processedResponse.Id);

        // Retrieve the payment using its ID.
        GetPaymentByIdQuery getQuery = new(processedResponse.Id);
        PaymentResponseDto retrievedResponse = await getHandler.Handle(getQuery, CancellationToken.None);

        // Assert:
        Assert.Equal(processedResponse.Id, retrievedResponse.Id);
        Assert.Equal(processedResponse.Status, retrievedResponse.Status);
        Assert.Equal(processedResponse.LastFourCardDigits, retrievedResponse.LastFourCardDigits);
        Assert.Equal(processedResponse.ExpiryMonth, retrievedResponse.ExpiryMonth);
        Assert.Equal(processedResponse.ExpiryYear, retrievedResponse.ExpiryYear);
        Assert.Equal(processedResponse.Currency, retrievedResponse.Currency);
        Assert.Equal(processedResponse.Amount, retrievedResponse.Amount);
    }
}