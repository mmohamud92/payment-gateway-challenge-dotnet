using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.UnitTests.Entities;

public class PaymentTests
{
    private readonly int _validMonth = DateTime.UtcNow.Month;
    private readonly int _validYear = DateTime.UtcNow.Year + 1;

    // Helper method to create a valid Payment using the new constructor.
    private Payment CreateValidPayment()
    {
        Guid merchantId = Guid.NewGuid();
        string validCardNumber = "4111111111111111"; // example 16-digit card number
        int amount = 10000;
        string currency = "GBP";
        string cvv = "123";

        return new Payment(merchantId, validCardNumber, _validMonth, _validYear, amount, currency, cvv);
    }

    [Fact]
    public void Constructor_WithValidInputs_ShouldInitialiseCorrectly()
    {
        // Arrange
        Guid merchantId = Guid.NewGuid();
        string validCardNumber = "4111111111111111";
        int amount = 10000;
        string currency = "GBP";
        string cvv = "123";

        // Act
        Payment payment = new(merchantId, validCardNumber, _validMonth, _validYear, amount, currency, cvv);

        // Assert
        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(merchantId, payment.MerchantId);
        Assert.Equal(amount, payment.Denomination.Amount);
        Assert.Equal(currency, payment.Denomination.Currency.ToString());
        // Validate that the LastFourDigits property is set correctly.
        string expectedLastFour = validCardNumber.Substring(validCardNumber.Length - 4);
        Assert.EndsWith(expectedLastFour, payment.LastFourDigits);
        Assert.True((DateTime.UtcNow - payment.Timestamp).TotalSeconds < 5, "Timestamp should be recent.");
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        Guid merchantId = Guid.NewGuid();
        string validCardNumber = "4111111111111111";
        int negativeAmount = -100; // invalid
        string currency = "GBP";
        string cvv = "123";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new Payment(merchantId, validCardNumber, _validMonth, _validYear, negativeAmount, currency, cvv));
        Assert.Contains("Amount cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidMerchantId_ShouldThrowArgumentException()
    {
        // Arrange
        Guid invalidMerchantId = Guid.Empty;
        string validCardNumber = "4111111111111111";
        int amount = 10000;
        string currency = "GBP";
        string cvv = "123";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new Payment(invalidMerchantId, validCardNumber, _validMonth, _validYear, amount, currency, cvv));
        Assert.Contains("Merchant ID is required.", exception.Message);
    }

    [Theory]
    [InlineData(PaymentStatus.Authorised)]
    [InlineData(PaymentStatus.Declined)]
    public void UpdateStatus_ToAuthorisedOrDeclined_WhenStatusIsPending_ShouldChangeStatus(PaymentStatus status)
    {
        // Arrange
        Payment payment = CreateValidPayment();

        // Act
        payment.UpdateStatus(status);

        // Assert
        Assert.Equal(status, payment.Status);
    }

    [Fact]
    public void UpdateStatus_WhenStatusIsNotPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        payment.UpdateStatus(PaymentStatus.Authorised);

        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => payment.UpdateStatus(PaymentStatus.Declined));
        Assert.Contains("Only payments in the Pending state can be updated.", exception.Message);
    }

    [Fact]
    public void UpdateStatus_ToSameStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Payment payment = CreateValidPayment();

        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => payment.UpdateStatus(PaymentStatus.Pending));
        Assert.Contains("Payment status is already set to this value.", exception.Message);
    }

    [Fact]
    public void UpdateStatus_ToInvalidStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Payment payment = CreateValidPayment();

        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => payment.UpdateStatus((PaymentStatus)99));
        Assert.Contains("Invalid status transition. Payments can only transition from Pending to Authorised or Declined.",
            exception.Message);
    }

    [Fact]
    public void SetAuthorisationCode_WhenPaymentIsAuthorised_ShouldSetCode()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        payment.UpdateStatus(PaymentStatus.Authorised);
        string expectedCode = "AUTH123";

        // Act
        payment.SetAuthorisationCode(expectedCode);

        // Assert
        Assert.Equal(expectedCode, payment.AuthorisationCode);
    }

    [Fact]
    public void SetAuthorisationCode_WhenPaymentIsDeclined_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Payment payment = CreateValidPayment();
        payment.UpdateStatus(PaymentStatus.Declined);

        // Act & Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => payment.SetAuthorisationCode("AUTH123"));
        Assert.Contains("Authorization code can only be set when the payment is authorised.", exception.Message);
    }
}