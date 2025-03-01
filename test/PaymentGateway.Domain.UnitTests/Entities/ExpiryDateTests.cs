using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

[Trait("Category", "Unit")]
public class ExpiryDateTests
{
    private readonly string _validExpiryYear = (DateTime.UtcNow.Year + 1).ToString();
    private const string ValidExpiryMonth = "10";

    [Fact]
    public void Constructor_WithValidDate_ShouldInitialiseCorrectly()
    {
        // Arrange & Act
        ExpiryDate expiryDate = new(ValidExpiryMonth, _validExpiryYear);

        // Assert
        Assert.Equal(_validExpiryYear, expiryDate.Year.ToString());
        Assert.Equal(ValidExpiryMonth, expiryDate.PaddedMonth);
    }

    [Fact]
    public void Constructor_WithOneDigitMonth_ShouldConvertAndInitialiseCorrectly()
    {
        // Arrange
        const string month = "1";
        const string paddedMonth = "0" + month;

        // Act
        ExpiryDate expiryDate = new(month, _validExpiryYear);

        // Assert
        Assert.Equal(_validExpiryYear, expiryDate.Year.ToString());
        Assert.Equal(paddedMonth, expiryDate.PaddedMonth);
    }

    [Fact]
    public void Constructor_WithTwoDigitYear_ShouldConvertAndInitialiseCorrectly()
    {
        // Arrange
        string twoDigitYear = _validExpiryYear.Substring(_validExpiryYear.Length - 2);
        int currentCentury = DateTime.UtcNow.Year / 100;
        int expectedYear = currentCentury * 100 + int.Parse(twoDigitYear);

        // Act
        ExpiryDate expiryDate = new(ValidExpiryMonth, twoDigitYear);

        // Assert
        Assert.Equal(expectedYear, expiryDate.Year);
        Assert.Equal(int.Parse(ValidExpiryMonth), expiryDate.Month);
    }

    [Fact]
    public void Constructor_WithExpiredYear_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        string invalidExpiryYear = (DateTime.UtcNow.Year - 1).ToString();

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate("12", invalidExpiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Fact]
    public void Constructor_WithCurrentYearAndExpiredMonth_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        string expiryYear = $"{DateTime.UtcNow.Year}";
        string invalidExpiryMonth = $"{DateTime.UtcNow.Month - 1}";

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("13")]
    public void Constructor_WithInvalidExpiryMonth_ShouldThrowInvalidExpiryDateException(string invalidExpiryMonth)
    {
        // Arrange
        string expiryYear = _validExpiryYear;

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry month must be between 01 and 12.", exception.Message);
    }
}