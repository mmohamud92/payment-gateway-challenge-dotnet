using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Infrastructure.UnitTests.Persistence;

public class PaymentRepositoryTests
{
    private readonly int _validMonth = DateTime.UtcNow.Month;
    private readonly int _validYear = DateTime.UtcNow.Year + 1;

    private readonly IPaymentRepository _paymentRepository;

    public PaymentRepositoryTests()
    {
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        _paymentRepository = new PaymentRepository(memoryCache);
    }

    [Fact]
    public void AddPayment_ShouldStorePaymentSuccessfully()
    {
        // Arrange
        Payment payment = CreateValidPayment();

        // Act
        _paymentRepository.AddPayment(payment);

        // Assert
        Payment? retrievedPayment = _paymentRepository.GetPaymentById(payment.Id);
        Assert.Equal(payment, retrievedPayment);
    }

    [Fact]
    public void AddPayment_ShouldThrowException_WhenPaymentAlreadyExists()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        // Act & Assert
        DuplicatePaymentException exception = Assert.Throws<DuplicatePaymentException>(
            () => _paymentRepository.AddPayment(payment));
        Assert.Contains($"Payment already exists for payment id: {payment.Id}", exception.Message);
    }

    [Fact]
    public void GetPaymentById_ShouldReturnPayment_WhenExists()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        // Act
        Payment? retrievedPayment = _paymentRepository.GetPaymentById(payment.Id);

        // Assert
        Assert.NotNull(retrievedPayment);
        Assert.Equal(payment.Id, retrievedPayment.Id);
        Assert.Equal(payment.Denomination, retrievedPayment.Denomination);
        Assert.Equal(payment.MerchantId, retrievedPayment.MerchantId);
        Assert.Equal(payment.AuthorisationCode, retrievedPayment.AuthorisationCode);
    }

    [Fact]
    public void GetPaymentById_ShouldThrowException_WhenPaymentDoesNotExist()
    {
        // Arrange
        Guid invalidPaymentId = Guid.NewGuid();

        // Act & Assert
        PaymentNotFoundException exception = Assert.Throws<PaymentNotFoundException>(
            () => _paymentRepository.GetPaymentById(invalidPaymentId));
        Assert.Contains($"No payment found with the specified id: {invalidPaymentId}", exception.Message);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldUpdateStatus_WhenPaymentExists()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        // Act
        _paymentRepository.UpdatePaymentStatus(payment.Id, PaymentStatus.Authorised);

        // Assert
        Payment? updatedPayment = _paymentRepository.GetPaymentById(payment.Id);
        Assert.Equal(PaymentStatus.Authorised, updatedPayment!.Status);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldThrowException_WhenPaymentDoesNotExist()
    {
        // Arrange
        Guid invalidPaymentId = Guid.NewGuid();

        // Act & Assert
        PaymentNotFoundException exception = Assert.Throws<PaymentNotFoundException>(
            () => _paymentRepository.UpdatePaymentStatus(invalidPaymentId, PaymentStatus.Declined));
        Assert.Contains($"No payment found with the specified ID: {invalidPaymentId}", exception.Message);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldThrowException_WhenInvalidTransitionOccurs()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);
        _paymentRepository.UpdatePaymentStatus(payment.Id, PaymentStatus.Authorised);

        // Act & Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            _paymentRepository.UpdatePaymentStatus(payment.Id, PaymentStatus.Declined));
        Assert.Contains("Only payments in the Pending state can be updated.", exception.Message);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldThrowException_WhenSettingToSameStatus()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        // Act & Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => _paymentRepository.UpdatePaymentStatus(payment.Id, PaymentStatus.Pending));
        Assert.Contains("Payment status is already set to this value.", exception.Message);
    }

    [Fact]
    public void UpdatePaymentStatus_ShouldThrowException_WhenInvalidStatusProvided()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        _paymentRepository.AddPayment(payment);

        // Act & Assert 
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => _paymentRepository.UpdatePaymentStatus(payment.Id, (PaymentStatus)99));
        Assert.Contains("Invalid status transition. Payments can only transition from Pending to Authorised or Declined.",
            exception.Message);
    }

    private static Payment CreateValidPayment()
    {
        Guid merchantId = Guid.NewGuid();
        string validCardNumber = "4111111111111111"; // Example valid card number
        int amount = 100000;
        string currency = "GBP";
        string cvv = "123";
        int validMonth = DateTime.UtcNow.Month;
        int validYear = DateTime.UtcNow.Year + 1;

        return new Payment(merchantId, validCardNumber, validMonth, validYear, amount, currency, cvv);
    }
}