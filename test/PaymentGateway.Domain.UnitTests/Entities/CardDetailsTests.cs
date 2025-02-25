using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.UnitTests.Entities;

[Trait("Category", "Unit")]
public class CardDetailsTests
{
    private const string CardNumber = "1234567890123456";
    private const string CVV = "1234";
    private const int ExpiryMonth = 12;
    private readonly int _expiryYear = DateTime.UtcNow.Year + 1;

    [Fact]
    public void Constructor_WithValidInputs_ShouldInitializeCorrectly()
    {
        // Arrange
        CardNumber cardNumber = new(CardNumber);
        ExpiryDate expiryDate = new(ExpiryMonth, _expiryYear);
        Cvv cvv = new(CVV);

        // Act
        CardDetails cardDetails = new(cardNumber, expiryDate, cvv);

        // Assert
        Assert.Equal(cardNumber, cardDetails.CardNumber);
        Assert.Equal(expiryDate, cardDetails.ExpiryDate);
        Assert.Equal(cvv, cardDetails.Cvv);
    }

    [Fact]
    public void Constructor_WithNullCardNumber_ShouldThrowArgumentNullException()
    {
        // Arrange
        ExpiryDate expiryDate = new(ExpiryMonth, _expiryYear);
        Cvv cvv = new(CVV);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CardDetails(null, expiryDate, cvv));
    }

    [Fact]
    public void Constructor_WithNullExpiryDate_ShouldThrowArgumentNullException()
    {
        // Arrange
        CardNumber cardNumber = new(CardNumber);
        Cvv cvv = new(CVV);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CardDetails(cardNumber, null, cvv));
    }

    [Fact]
    public void Constructor_WithNullCVV_ShouldThrowArgumentNullException()
    {
        // Arrange
        CardNumber cardNumber = new(CardNumber);
        ExpiryDate expiryDate = new(ExpiryMonth, _expiryYear);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CardDetails(cardNumber, expiryDate, null));
    }
}