using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

public class CVVTests
{
    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Constructor_WithGoodCVV_ShouldInitialiseCorrectly(string cvv)
    {
        // Act
        Cvv goodCVV = new(cvv);

        // Assert
        Assert.Equal(cvv, goodCVV.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyCVV_ShouldThrowInvalidCVVException(string invalidCVV)
    {
        // Act & Assert
        InvalidCvvException exception = Assert.Throws<InvalidCvvException>(() => new Cvv(invalidCVV));
        Assert.Contains("CVV is required.", exception.Message);
    }

    [Theory]
    [InlineData("12")]
    [InlineData(" 12345")]
    [InlineData("a123")]
    public void Constructor_WithIncorrectCVV_ShouldThrowInvalidCVVException(string invalidCVV)
    {
        // Act & Assert
        InvalidCvvException exception = Assert.Throws<InvalidCvvException>(() => new Cvv(invalidCVV));
        Assert.Contains("CVV must be numeric and either 3 or 4 digits.", exception.Message);
    }
}