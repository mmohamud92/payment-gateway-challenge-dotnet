using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

[Trait("Category", "Unit")]
public class ExpiryDateTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(11)]
    public void Constructor_WithValidDate_ShouldInitialiseCorrectly(int month)
    {
        // Arrange
        int year = DateTime.UtcNow.Year + 1;

        //Act
        ExpiryDate expiryDate = new(month, year);

        // Assert
        Assert.Equal(year, expiryDate.Year);
        Assert.Equal(month, expiryDate.Month);
    }

    [Fact]
    public void Constructor_WithExpiredYear_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        int invalidExpiryYear = DateTime.UtcNow.Year - 1;

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(12, invalidExpiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Fact]
    public void Constructor_WithCurrentYearAndExpiredMonth_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        int expiryYear = DateTime.UtcNow.Year;
        int invalidExpiryMonth = DateTime.UtcNow.Month - 1;

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Constructor_WithInvalidExpiryMonth_ShouldThrowInvalidExpiryDateException(int invalidExpiryMonth)
    {
        // Arrange
        int expiryYear = DateTime.UtcNow.Year + 1;

        // Act & Assert
        InvalidExpiryDateException exception =
            Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry month must be between 1 and 12.", exception.Message);
    }
}