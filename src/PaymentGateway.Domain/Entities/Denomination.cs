using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;

namespace PaymentGateway.Domain.Entities;

public class Denomination
{
    public int Amount { get; }
    public Currency Currency { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Denomination"/> with validation.
    /// </summary>
    /// <param name="amount">The monetary amount (must be non-negative).</param>
    /// <param name="currencyCode">The currency code as a string (e.g., "USD").</param>
    /// <exception cref="ArgumentException">Thrown if currency is invalid or amount is negative.</exception>
    public Denomination(int amount, string currencyCode)
    {
        if (amount < 0)
        {
            throw new PaymentValidationException("Amount cannot be negative.");
        }

        if (!TryGetCurrency(currencyCode, out Currency parsedCurrency))
        {
            throw new PaymentValidationException($"Invalid currency code: {currencyCode}");
        }

        Amount = amount;
        Currency = parsedCurrency;
    }

    /// <summary>
    /// Attempts to convert a string currency code into a <see cref="Currency"/> enum.
    /// </summary>
    private static bool TryGetCurrency(string currency, out Currency result)
    {
        return Enum.TryParse(currency, true, out result) && Enum.IsDefined(result);
    }
}