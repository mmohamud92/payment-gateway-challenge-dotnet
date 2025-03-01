using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.UnitTests.Entities;

[Trait("Category", "Unit")]
public class DenominationTests
{
    [Theory]
    [InlineData(100, "USD")]
    [InlineData(50.75, "EUR")]
    [InlineData(200, "GBP")]
    [InlineData(99.99, "EUR")]
    [InlineData(0, "GBP")]
    public void Constructor_WithValidAmountAndCurrency_ShouldInitialiseCorrectly(int amount, string currencyCode)
    {
        // Act
        Denomination denomination = new(amount, currencyCode);

        // Assert
        Assert.Equal(amount, denomination.Amount);
        Assert.Equal(Enum.Parse<Currency>(currencyCode), denomination.Currency);
    }

    [Fact]
    public void Constructor_WithLowercaseCurrency_ShouldBeCaseInsensitive()
    {
        // Act
        Denomination denomination = new(50, "usd");

        // Assert
        Assert.Equal(Currency.USD, denomination.Currency);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowPaymentValidationException()
    {
        // Act & Assert
        PaymentValidationException exception = Assert.Throws<PaymentValidationException>(
            () => new Denomination(-10, "USD"));

        Assert.Contains("Amount cannot be negative.", exception.Message);
    }

    [Theory]
    [InlineData("JPY")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData("usdollar")]
    public void Constructor_WithInvalidCurrency_ShouldThrowPaymentValidationException(string invalidCurrency)
    {
        // Act & Assert
        PaymentValidationException exception = Assert.Throws<PaymentValidationException>(
            () => new Denomination(100, invalidCurrency));

        Assert.Contains($"Invalid currency code: {invalidCurrency}", exception.Message);
    }
}