using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

public class ExpiryDateTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(11)]
    public void Constructor_WithValidDate_ShouldInitialiseCorrectly(int month)
    {
        // Arrange
        var year = DateTime.UtcNow.Year + 1;
        
        //Act
        var expiryDate = new ExpiryDate(month, year);
        
        // Assert
        Assert.Equal(year, expiryDate.Year);
        Assert.Equal(month, expiryDate.Month);
    }
    
    [Fact]
    public void Constructor_WithExpiredYear_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        var invalidExpiryYear = DateTime.UtcNow.Year - 1;
        
        // Act & Assert
        var exception = Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(12, invalidExpiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Fact]
    public void Constructor_WithCurrentYearAndExpiredMonth_ShouldThrowInvalidExpiryDateException()
    {
        // Arrange
        var expiryYear = DateTime.UtcNow.Year;
        var invalidExpiryMonth = DateTime.UtcNow.Month - 1;
        
        // Act & Assert
        var exception = Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry date must be in the future.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Constructor_WithInvalidExpiryMonth_ShouldThrowInvalidExpiryDateException(int invalidExpiryMonth)
    {
        // Arrange
        var expiryYear = DateTime.UtcNow.Year + 1;
        
        // Act & Assert
        var exception = Assert.Throws<InvalidExpiryDateException>(() => new ExpiryDate(invalidExpiryMonth, expiryYear));
        Assert.Contains("The expiry month must be between 1 and 12.", exception.Message);
    }
}