using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

public class CardNumberTests
{
    [Theory]
    [InlineData("4823157692345678")]
    [InlineData("6589741236500987")]
    [InlineData("9023 4567 8932 1256")]
    [InlineData("90234567 89321256")]
    public void Constructor_WithValidCardNumbers_ShouldInitialiseCorrectly(string validCardNumber)
    {
        // Act
        CardNumber cardNumber = new(validCardNumber);

        // Assert
        Assert.Equal(validCardNumber.Replace(" ", ""), cardNumber.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyCardNumber_ShouldThrowInvalidCardNumberException(string invalidCardNumber)
    {
        // Act & Assert
        InvalidCardNumberException exception =
            Assert.Throws<InvalidCardNumberException>(() => new CardNumber(invalidCardNumber));
        Assert.Contains("Card number is required.", exception.Message);
    }

    [Theory]
    [InlineData("1234567890123")]
    [InlineData("12345678901234567890")]
    [InlineData("12345678901234ab")]
    [InlineData("1234567890 1234567890")]
    public void Constructor_WithInvalidCardNumbers_ShouldThrowInvalidCardNumberException(string invalidCardNumber)
    {
        // Act & Assert
        InvalidCardNumberException exception =
            Assert.Throws<InvalidCardNumberException>(() => new CardNumber(invalidCardNumber));
        Assert.Contains("Card number must be numeric and between 14 and 19 digits.", exception.Message);
    }
}